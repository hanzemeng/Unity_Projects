using UnityEngine;
using EmeraldAI;

public class PukeStormProjectile : MonoBehaviour
{
    public AudioManager.SoundResource impactSound;
    public GameObject impactExplosion;
    public Vector3 fallingDirection; // must be normalized
    public float fallingSpeed;
    public int damage;

    private void FixedUpdate()
    {
        transform.position += Time.deltaTime*fallingSpeed*fallingDirection;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(null != collider.GetComponent<Neuron>() || null != collider.GetComponent<SpellAOE>() || "Axorath(Clone)" == collider.gameObject.name)
        {
            return;
        }
        if(null != collider.transform.GetComponent<EmeraldAI.EmeraldAISystem>())
        {
            collider.transform.GetComponent<EmeraldAI.EmeraldAISystem>().Damage(damage, EmeraldAISystem.TargetType.AI, transform, 100);
        }
        else if(null != collider.transform.GetComponent<EmeraldAI.EmeraldAIPlayerDamage>())
        {
            collider.transform.GetComponent<EmeraldAI.EmeraldAIPlayerDamage>().SendPlayerDamage(damage, transform, GetComponent<EmeraldAISystem>());
        }

        AudioManager.instance.PlayBackgroundSFX(impactSound);
        Instantiate(impactExplosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
