using UnityEngine;

[RequireComponent(typeof(ObjectPermanent))]
public class SpellCollectible : MonoBehaviour
{
    public SpellType spellToAdd;
    [SerializeField] private GameObject collectParticleFX;
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag != Neuromancer.Unit.HERO_TAG) { return; }
        PlayerInventory.current.AddSpell(spellToAdd);
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.PLAYER_MISSILE_LAUNCH);
        Instantiate(collectParticleFX, transform.position, Quaternion.identity);
        GetComponent<ObjectPermanent>().SetDestroy();
        Destroy(gameObject);
    }
}
