using UnityEngine;
using EmeraldAI;

public class Pukeball : MonoBehaviour
{
    public int damage;
    public float damageRadius;
    private void OnTriggerEnter(Collider other)
    {
        int layerMask = 0;
        layerMask |= 1 << LayerMask.NameToLayer("Ally");
        layerMask |= 1 << LayerMask.NameToLayer("Hero");
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius, layerMask);
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
