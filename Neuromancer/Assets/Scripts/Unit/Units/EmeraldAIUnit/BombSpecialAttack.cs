using System.Collections;
using EmeraldAI;
using UnityEngine;

public class BombSpecialAttack : MonoBehaviour
{
    public float explosionDelay;
    public int damage;
    public float radius;

    public GameObject explosionIndicationPrefab;
    public GameObject explosionPrefab;

    private void Start()
    {
        if(LayerMask.NameToLayer("Enemy") == gameObject.layer)
        {
            GetComponent<EmeraldAISystem>().MeleeAttacks[0].AttackOdds = 1000;
        }
        else
        {
            GetComponent<EmeraldAISystem>().MeleeAttacks[0].AttackOdds = 0;
            GetComponent<EmeraldAISystem>().MeleeAttacks[1].AttackOdds = 1000;
        }
    }

    public void Explode()
    {
        StartCoroutine(ExplodeCoroutine());
    }

    private IEnumerator ExplodeCoroutine()
    {
        GetComponent<EmeraldAI.EmeraldAISystem>().DeathEvent.Invoke();
        GetComponent<EmeraldAISystem>().enabled = false;
        GetComponent<EmeraldAIBehaviors>().enabled = false;
        GetComponent<Animator>().enabled = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<UnitMentalStamina>().enabled = false;
        GameObject explosionIndication = Instantiate(explosionIndicationPrefab, transform.position+1.5f*Vector3.up, Quaternion.identity);
        explosionIndication.transform.SetParent(transform, true);

        yield return new WaitForSeconds(explosionDelay);
        Destroy(gameObject);
        Destroy(explosionIndication);
        Instantiate(explosionPrefab, transform.position+Vector3.up, Quaternion.identity);

        int layerMask = 0;
        layerMask |= 1 << LayerMask.NameToLayer("Enemy");
        layerMask |= 1 << LayerMask.NameToLayer("Ally");
        layerMask |= 1 << LayerMask.NameToLayer("Hero");
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);
        foreach(Collider collider in colliders)
        {
            if(collider.gameObject == gameObject)
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
    }
}
