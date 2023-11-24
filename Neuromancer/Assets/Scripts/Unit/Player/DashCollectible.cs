using UnityEngine;

[RequireComponent(typeof(ObjectPermanent))]
public class DashCollectible : MonoBehaviour
{
    [SerializeField] private GameObject collectParticleFX;
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag != Neuromancer.Unit.HERO_TAG) { return; }
        PlayerController.player.EnableDash(true);
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.PLAYER_MISSILE_LAUNCH);
        Instantiate(collectParticleFX, transform.position, Quaternion.identity);
        GetComponent<ObjectPermanent>().SetDestroy();
        Destroy(gameObject);
    }
}
