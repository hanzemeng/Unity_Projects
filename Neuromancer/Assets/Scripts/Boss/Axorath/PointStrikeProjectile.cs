using UnityEngine;
using EmeraldAI;

public class PointStrikeProjectile : MonoBehaviour
{
    public AudioManager.SoundResource impactSound;
    public GameObject impactExplosion;
    public Vector3 impactExplosionOffset;
    public Vector3 fallingDirection; // must be normalized
    public float fallingSpeed;
    public float radius;
    public int damage;

    private void FixedUpdate()
    {
        transform.position += Time.deltaTime*fallingSpeed*fallingDirection;
    }

    private void OnTriggerEnter(Collider c)
    {
        if(c.gameObject.layer != LayerMask.NameToLayer("Ground"))
        {
            return;
        }

        int layerMask = 0;
        layerMask |= 1 << LayerMask.NameToLayer("Enemy");
        layerMask |= 1 << LayerMask.NameToLayer("Ally");
        layerMask |= 1 << LayerMask.NameToLayer("Hero");
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);
        foreach(Collider collider in colliders)
        {
            if(collider.gameObject == gameObject || collider.gameObject.name == "Axorath(Clone)")
            {
                continue;
            }

            if(null != collider.transform.GetComponent<EmeraldAI.EmeraldAISystem>())
            {
                collider.transform.GetComponent<EmeraldAI.EmeraldAISystem>().Damage(damage, EmeraldAISystem.TargetType.AI, transform, 100);
            }
            else if(null != collider.transform.GetComponent<EmeraldAI.EmeraldAIPlayerDamage>())
            {
                collider.transform.GetComponent<EmeraldAI.EmeraldAIPlayerDamage>().SendPlayerDamage(damage, transform, GetComponent<EmeraldAISystem>());
            }
        }

        AudioManager.instance.PlayBackgroundSFX(impactSound);
        Instantiate(impactExplosion, transform.position + impactExplosionOffset, Quaternion.identity);
        Destroy(gameObject);
    }
}
