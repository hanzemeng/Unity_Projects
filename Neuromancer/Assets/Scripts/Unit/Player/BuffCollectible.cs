using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ObjectPermanent))]
public class BuffCollectible : MonoBehaviour
{
    public Buff buffToAdd;
    private Image icon;
    private Canvas canvas;

    private void Awake() { // Set the color of the particle effects and icon to match the buff it holds
        ParticleSystem sparks = transform.GetChild(0).GetComponent<ParticleSystem>();
        ParticleSystem glow = transform.GetChild(1).GetComponent<ParticleSystem>();
        ParticleSystem sphere = transform.GetChild(2).GetComponent<ParticleSystem>();

        Color myColor = UIConstants.buffColors[(int)buffToAdd.buffType];
        
        ParticleSystem.ColorOverLifetimeModule sparksColor = sparks.colorOverLifetime;
        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(myColor, 1.0f) },
                     new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) });
        sparksColor.color = grad;

        ParticleSystem.MainModule glowColor = glow.main;
        glowColor.startColor = myColor;

        ParticleSystem.ColorOverLifetimeModule sphereColor = sphere.colorOverLifetime;
        grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(myColor, 0.5f), new GradientColorKey(Color.white, 1.0f) },
                     new GradientAlphaKey[] { new GradientAlphaKey(0.25f, 0.0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0.25f, 1.0f) });
        sphereColor.color = grad;

        icon = transform.GetChild(3).GetChild(0).GetComponent<Image>();
        icon.overrideSprite = buffToAdd.icon;
        canvas = icon.GetComponentInParent<Canvas>();
    }

    private void LateUpdate() {
        canvas.transform.rotation = Camera.main.transform.rotation;
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag != Neuromancer.Unit.HERO_TAG) { return; }
        bool success = PlayerInventory.current.AddBuff(buffToAdd.token);
        if (!success) { return; }
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.PLAYER_MISSILE_LAUNCH);
        Instantiate(buffToAdd.collectParticle, transform.position, Quaternion.identity);
        GetComponent<ObjectPermanent>().SetDestroy();
        Destroy(gameObject);
    }
}
