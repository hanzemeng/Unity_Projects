using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellCone : MonoBehaviour
{
    private ConeSpell spell;
    private Transform head;
    private Vector3 end;
    private Vector3 direction;
    private GameObject spellSound;
    private List<Transform> targets = new List<Transform>();
    private List<Transform> allTargets = new List<Transform>();
    private int TARGET_MASK;

    // Copied and modified from PolygonBeamStatic.cs
    [Header("Prefabs")]
    [SerializeField] private GameObject beamLineRendererPrefab; //Put a prefab with a line renderer onto here.
    [SerializeField] private GameObject beamStartPrefab; //This is a prefab that is put at the start of the beam.
    [SerializeField] private GameObject beamEndPrefab; //Prefab put at end of beam.

    private GameObject beamStart;
    private GameObject beamEnd;
    private GameObject beam;
    private LineRenderer line;
    private float baseWidth = 1f;

    [Header("Beam Options")]
    [SerializeField] private float beamLength = 3.5f; //Ingame beam length
    [SerializeField] private float textureScrollSpeed = 0f; //How fast the texture scrolls along the beam, can be negative or positive.
    [SerializeField] private float textureLengthScale = 1f;   //Set this to the horizontal length of your texture relative to the vertical. 
                                            //Example: if texture is 200 pixels in height and 600 in length, set this to 3

    public void Initialize(ConeSpell spell, Transform head) {
        this.spell = spell;
        this.head = head;
        spellSound = AudioManager.instance.MusicAttached(AudioManager.SoundResource.PLAYER_SPELL_ACTIVE, transform);
        AudioManager.instance.PlayMusic(spellSound);
        TARGET_MASK = LayerMask.GetMask(Neuromancer.Unit.HERO_LAYER_NAME, Neuromancer.Unit.ENEMY_LAYER_NAME, Neuromancer.Unit.ALLY_LAYER_NAME);
        SpawnBeam();
    }

    public void ModWidth(float scale) {
        line.endWidth = baseWidth * scale;
    }

    private void Update() {
        transform.position = head.position;
        line.SetPosition(0, line.transform.InverseTransformPoint(head.position));
        Vector2 mousePos = PlayerInputManager.playerInputs.CameraAction.CursorPosition.ReadValue<Vector2>();
        Vector3 endPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.WorldToScreenPoint(transform.position).z));
        endPos.y = transform.position.y;
        direction = (endPos - transform.position).normalized;
        end = transform.position + (direction * beamLength);
        line.SetPosition(1, line.transform.InverseTransformPoint(end));
        if (beamStart)
        {
            beamStart.transform.position = transform.position;
            beamStart.transform.LookAt(end);
        }
        if (beamEnd)
        {
            beamEnd.transform.position = end + (direction * line.endWidth / 2f);
            beamEnd.transform.LookAt(beamStart.transform.position);
        }
        float distance = Vector3.Distance(transform.position, end);
        line.material.mainTextureScale = new Vector2(distance / textureLengthScale, 1); //This sets the scale of the texture so it doesn't look stretched
        line.material.mainTextureOffset -= new Vector2(Time.deltaTime * textureScrollSpeed, 0); //This scrolls the texture along the beam if not set to 0

        GetTargets();
        foreach(Transform t in targets) {
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
            line = beam.GetComponent<LineRenderer>();
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
        Vector3 vertex1 = head.position; // the head
        Vector3 vertex2 = end + direction * line.endWidth / 2f; // the peak end point
        Vector3 cross = Vector3.Cross(direction, Vector3.up);
        Vector3 vertex3 = end + cross * line.endWidth / 2f;
        Vector3 vertex4 = end - cross * line.endWidth / 2f; // left and right vertices
        Vector3 vertex5 = (vertex2 + vertex3) / 2f;
        Vector3 vertex6 = (vertex2 + vertex4) / 2f; // horizontal left and right midpoints
        Vector3 vertex7 = (vertex1 + vertex3) / 2f;
        Vector3 vertex8 = (vertex1 + vertex4) / 2f; // vertical left and right midpoints

        Vector3 center = (vertex1 + vertex2) / 2f;
        Vector3 dimension = new Vector3(Vector3.Distance(vertex3, vertex4) / 2f, 9f, Vector3.Distance(vertex1, center) + 0.05f);
        Collider[] cols = Physics.OverlapBox(center, dimension, Quaternion.LookRotation(direction), TARGET_MASK);
        foreach (Collider c in cols) { targets.Add(c.transform); }

        Vector3 cross13 = Vector3.Cross(vertex3 - vertex1, Vector3.up).normalized;
        Vector3 cross14 = Vector3.Cross(vertex1 - vertex4, Vector3.up).normalized;
        Vector3 cross23 = Vector3.Cross(vertex2 - vertex3, Vector3.up).normalized;
        Vector3 cross24 = Vector3.Cross(vertex4 - vertex2, Vector3.up).normalized;
        float r1 = Vector3.Distance(vertex1, vertex3) / 2f;
        float r2 = Vector3.Distance(vertex2, vertex3) / 2f;
        Vector3 c13 = vertex7 + cross13 * r1;
        Vector3 c14 = vertex8 + cross14 * r1;
        Vector3 c23 = vertex5 + cross23 * r2;
        Vector3 c24 = vertex6 + cross24 * r2;
        Vector3 d1 = new Vector3(r1 / 2f, 7f, r1 / 2f);
        Vector3 d2 = new Vector3(r2 / 2f, 7f, r2 / 2f);
        Collider[] mask13 = Physics.OverlapBox(c13, d1, Quaternion.LookRotation(c13), TARGET_MASK);
        Collider[] mask14 = Physics.OverlapBox(c14, d1, Quaternion.LookRotation(c14), TARGET_MASK);
        Collider[] mask23 = Physics.OverlapBox(c23, d2, Quaternion.LookRotation(c23), TARGET_MASK);
        Collider[] mask24 = Physics.OverlapBox(c24, d2, Quaternion.LookRotation(c24), TARGET_MASK);
        List<Transform> masks = new List<Transform>();
        foreach (Collider c in mask13) { masks.Add(c.transform); }
        foreach (Collider c in mask14) { masks.Add(c.transform); }
        foreach (Collider c in mask23) { masks.Add(c.transform); }
        foreach (Collider c in mask24) { masks.Add(c.transform); }
        // Remove items that occur multiple times. It's probably a very big collider
        masks.GroupBy(i => i)
             .Where(g => g.Count() == 1)
             .Select(g => g.First());
        foreach (Transform t in masks) {
            if (targets.Contains(t)) { targets.Remove(t); }
        }
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
