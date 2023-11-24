using System.Collections.Generic;
using UnityEngine;

public class ItemDropController : MonoBehaviour
{
    private ParticleSystem part;
    private ParticleSystemRenderer particleSystemRenderer;

    public GameObject item;
    public LayerMask targetLayer;
    public int count;
    [HideInInspector] public string uniqueId;

    private bool hasParticlePos = false;
    private Vector3 particlePos;
    private ParticleSystem.Particle[] particles;

    public void Start()
    {
        part = GetComponent<ParticleSystem>();
        if (part != null)
            particleSystemRenderer = GetComponent<ParticleSystem>().GetComponent<ParticleSystemRenderer>();
        if (particleSystemRenderer == null)
        {
            Debug.LogError("Death Effect does not have particle system for item drops. Destroying particle system");
            Destroy(gameObject);
            return;
        }
        if (item != null)
        {
            particleSystemRenderer.mesh = item.GetComponent<MeshFilter>()?.sharedMesh;
            particleSystemRenderer.material = item.GetComponent<MeshRenderer>()?.sharedMaterial;
        }

        particles = new ParticleSystem.Particle[100];
    }


    private void OnParticleCollision(GameObject other)
    {
        int numberOfParticles = part.GetParticles(particles);
        if (numberOfParticles > 0)
        {
            hasParticlePos = true;
            particlePos = particles[0].position;
        }

    }

    //  GOAL : When particle effect reaches a trigger zone within its own gameObject, emit a large Physics.raycast sphere to find any gameObject
    private void OnParticleTrigger()
    {

        List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();
        int numInsideTrigger = part.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);

        if(numInsideTrigger > 0)
        {
            Collider[] allTriggerColliders = Physics.OverlapSphere(inside[0].position, part.trigger.radiusScale, targetLayer);

            if(allTriggerColliders.Length > 0)
            {
                Debug.Log("Found the valid trigger colliders with the target layer!");
            }

        }
    }

    private void OnParticleSystemStopped()
    {
        SpawnItem(item, count, hasParticlePos ? particlePos : transform.position);
        Destroy(gameObject, 0.5f);
    }

    private void SpawnItem(GameObject item, int count, Vector3 position)
    {
        if (item == null)
        {
            Debug.LogError("Item data missing. Abort");
            return;
        }
        GameObject newObject = Instantiate(item, position, Quaternion.identity);
        CarriableItem carriableItem = newObject.GetComponent<CarriableItem>();
        carriableItem.count = count;
        carriableItem.SetID(uniqueId);
    }
}
