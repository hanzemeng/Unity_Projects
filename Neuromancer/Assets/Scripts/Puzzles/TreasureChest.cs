using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
public class TreasureChest : MonoBehaviour
{
    private Animator animator;
    private bool isOpen;

    [SerializeField] private GameObject neuronPrefab;
    [SerializeField] private Transform neuronSpawnTransform;
    [SerializeField] private Transform neuronLandTransform;
    [SerializeField] private int neuronCount;
    [SerializeField] private float neuronBeginSpawnDelay;
    [SerializeField] private float neuronSpawnDelay;
    [SerializeField] private float neuronFlyHeight;
    [SerializeField] private float neuronSpawnVariance;
    [SerializeField] private float neuronLandVariance;

    private void Start()
    {
        animator = GetComponent<Animator>();
        isOpen = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(isOpen || (Neuromancer.Unit.HERO_TAG != other.gameObject.tag && Neuromancer.Unit.ALLY_LAYER_NAME != LayerMask.LayerToName(other.gameObject.layer)))
        {
            return;
        }
        isOpen = true;
        StartCoroutine(OpenChestCoroutine());
    }

    private IEnumerator OpenChestCoroutine()
    {
        animator.Play("ChestOpen", 0, 0.0f);
        yield return new WaitForSeconds(neuronBeginSpawnDelay);


        for(int i=0; i<neuronCount; i++)
        {
            GameObject temp = Instantiate(neuronPrefab, neuronSpawnTransform.position, neuronSpawnTransform.rotation);
            Collider[] colliders = temp.GetComponents<Collider>();
            foreach(Collider collider in colliders)
            {
                collider.enabled = false;
            }

            StartCoroutine(NeuronFlyCoroutine(temp.transform));

            yield return new WaitForSeconds(neuronSpawnDelay);
        }
    }

    private IEnumerator NeuronFlyCoroutine(Transform neuron)
    {
        float lerpAmount = 0;
        Vector2 temp = Random.insideUnitCircle;
        Vector3 source = neuron.position + neuronSpawnVariance*new Vector3(temp.x, 0f, temp.y);
        temp = Random.insideUnitCircle;
        Vector3 destination = neuronLandTransform.position + neuronLandVariance*new Vector3(temp.x, 0f, temp.y);
        while(lerpAmount < 1f)
        {
            lerpAmount += 1.5f*Time.deltaTime;
            neuron.position = Vector3.Lerp(source, destination, lerpAmount);
            neuron.position = new Vector3(neuron.position.x, destination.y - neuronFlyHeight*lerpAmount*lerpAmount + neuronFlyHeight*lerpAmount, neuron.position.z);
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        neuron.GetComponent<Neuron>().FlyToTarget(PlayerController.player.transform);
    }
}
