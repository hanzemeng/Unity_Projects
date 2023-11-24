using System.Collections.Generic;
using UnityEngine;

public class NeuronDropController : MonoBehaviour
{
    private ParticleSystem part;
    private ParticleSystemRenderer particleSystemRenderer;

    public GameObject item;
    private int count = 1;

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
            Debug.LogError("Neuron Effect does not have particle system for item drops. Destroying particle system");
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

    private void OnParticleSystemStopped()
    {
        SpawnItem(item, count, hasParticlePos ? particlePos : transform.position);
    }

    private void SpawnItem(GameObject item, int count, Vector3 position)
    {
        if (item == null)
        {
            Debug.LogError("Item data missing. Abort");
            return;
        }
        Instantiate(item, position, Quaternion.identity);
        Destroy(gameObject, 0.5f);
    }
}
