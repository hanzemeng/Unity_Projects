using System.Collections.Generic;
using UnityEngine;

public class SpellBeam : MonoBehaviour
{
    private BeamSpell spell;
    private Transform head;
    private Vector3 end;
    private Vector3 direction;
    private GameObject spellSound;
    private int TARGET_MASK;
    private List<Transform> targets = new List<Transform>();
    private List<Transform> allTargets = new List<Transform>();

    // Copied and modified from PolygonBeamStatic.cs
    [Header("Prefabs")]
    [SerializeField] private GameObject beamLineRendererPrefab; //Put a prefab with a line renderer onto here.
    [SerializeField] private GameObject beamStartPrefab; //This is a prefab that is put at the start of the beam.
    [SerializeField] private GameObject beamEndPrefab; //Prefab put at end of beam.

    private GameObject beamStart;
    private GameObject beamEnd;
    private GameObject beam;
    private LineRenderer line;
    private float baseLength;

    [Header("Beam Options")]
    [SerializeField] private float beamLength = 3.5f; //Ingame beam length
    [SerializeField] private float textureScrollSpeed = 0f; //How fast the texture scrolls along the beam, can be negative or positive.
    [SerializeField] private float textureLengthScale = 1f;   //Set this to the horizontal length of your texture relative to the vertical. 
                                            //Example: if texture is 200 pixels in height and 600 in length, set this to 3

    public void Initialize(BeamSpell spell, Transform head) {
        this.spell = spell;
        this.head = head;
        baseLength = beamLength;
        spellSound = AudioManager.instance.MusicAttached(AudioManager.SoundResource.PLAYER_SPELL_ACTIVE, transform);
        AudioManager.instance.PlayMusic(spellSound);
        TARGET_MASK = LayerMask.GetMask(Neuromancer.Unit.HERO_LAYER_NAME, Neuromancer.Unit.ENEMY_LAYER_NAME, Neuromancer.Unit.ALLY_LAYER_NAME);
        SpawnBeam();
    }

    public void ModLength(float scale) { beamLength = baseLength * scale; }

    private void Update() {
        transform.position = head.position;
        line.SetPosition(0, transform.position);
        Vector2 mousePos = PlayerInputManager.playerInputs.CameraAction.CursorPosition.ReadValue<Vector2>();
        Vector3 endPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.WorldToScreenPoint(transform.position).z));
        endPos.y = transform.position.y;
        direction = (endPos - transform.position).normalized;
        end = transform.position + (direction * beamLength);
        line.SetPosition(1, end);
        if (beamStart)
        {
            beamStart.transform.position = transform.position;
            beamStart.transform.LookAt(end);
        }
        if (beamEnd)
        {
            beamEnd.transform.position = end;
            beamEnd.transform.LookAt(beamStart.transform.position);
        }
        float distance = Vector3.Distance(transform.position, end);
        line.material.mainTextureScale = new Vector2(distance / textureLengthScale, 1); //This sets the scale of the texture so it doesn't look stretched
        line.material.mainTextureOffset -= new Vector2(Time.deltaTime * textureScrollSpeed, 0); //This scrolls the texture along the beam if not set to 0

        GetTargets();
        foreach (Transform t in targets) {
            if (Neuromancer.Unit.IsEnemy(t)) {
                UnitMentalStamina msController = t.GetComponent<UnitMentalStamina>();
                msController?.DrainMentalStamina(spell.spellSpecs.drainAmount * PlayerController.player.spellDamageModifier * Time.deltaTime);
                Neuromancer.NPCUnit npcUnit = t.GetComponent<Neuromancer.NPCUnit>();
                if (npcUnit != null && npcUnit.GetCurrentCommand().commandType != CommandType.ATTACK_TARGET && npcUnit.EmeraldComponent.CurrentTarget == null)
                {
                    npcUnit.IssueCommand(new Command(CommandType.ATTACK_TARGET, PlayerController.player.transform));
                }
            }
            if (Neuromancer.Unit.IsUnit(t) && !allTargets.Contains(t)) {
                List<Buff> buffs = spell.GetBuffs();
                if (buffs != null) {
                    foreach (Buff b in buffs) {
                        t.GetComponent<UnitStatus>()?.AddBuff(b, head);
                        t.GetComponent<PlayerStatus>()?.AddBuff(b, head);
                    }
                }
            }
        }
    }

    private void OnDisable() //If the object this script is attached to is disabled, remove the beam.
    {
        RemoveBuffs();
        AudioManager.instance.PauseMusic(spellSound);
        RemoveBeam();
    }

    private void SpawnBeam() //This function spawns the prefab with linerenderer
    {
        if (beamLineRendererPrefab)
        {
            if (beamStartPrefab)
                beamStart = Instantiate(beamStartPrefab);
            if (beamEndPrefab)
                beamEnd = Instantiate(beamEndPrefab);
            beam = Instantiate(beamLineRendererPrefab);
            beam.transform.position = transform.position;
            beam.transform.parent = transform;
            beam.transform.rotation = transform.rotation;
            line = beam.GetComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.positionCount = 2;
        }
        else
            print("Add a hecking prefab with a line renderer to the SciFiBeamStatic script on " + gameObject.name + "! Heck!");
    }

    private void RemoveBeam() //This function removes the prefab with linerenderer
    {
        if (beam)
            Destroy(beam);
        if (beamStart)
            Destroy(beamStart);
        if (beamEnd)
            Destroy(beamEnd);
    }

    private void GetTargets() {
        foreach (Transform t in targets) {
            if (!allTargets.Contains(t)) { allTargets.Add(t); }
        }
        targets.Clear();
        Vector3 midpoint = (head.position + end) / 2f;
        Vector3 dimension = new Vector3(0.3f, 7f, Vector3.Distance(head.position, midpoint) + 0.05f);
        Collider[] cols = Physics.OverlapBox(midpoint, dimension, Quaternion.LookRotation(direction), TARGET_MASK);
        foreach (Collider c in cols) { targets.Add(c.transform); }
        if (!targets.Contains(PlayerController.player.transform)) { targets.Add(PlayerController.player.transform); }
    }

    private void RemoveBuffs() {
        foreach (Transform t in allTargets) {
            if (t != null && Neuromancer.Unit.IsUnit(t)) {
                List<Buff> buffs = spell.GetBuffs();
                if (buffs != null) {
                    foreach (Buff b in buffs) {
                        t.GetComponent<UnitStatus>()?.RemoveBuff(b, 2f);
                        t.GetComponent<PlayerStatus>()?.RemoveBuff(b);
                    }
                }
            }
        }
    }
}
