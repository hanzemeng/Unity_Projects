using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMagic))]
public class NeuronCollectManager : MonoBehaviour
{
    // singleton for collection
    public static NeuronCollectManager neuronCollector;

    [SerializeField] private Transform collectFXTargetTransform;
    [SerializeField] private GameObject collectFX;
    private List<ParticleSystem> pooledCollectFXs = new List<ParticleSystem>();
    [SerializeField] private int amountToPool = 10;

    private UnitMagic playerMP;

    private void Awake()
    {
        if(neuronCollector == null)
        {
            neuronCollector = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        playerMP = GetComponent<UnitMagic>();

        collectFXTargetTransform = collectFXTargetTransform != null ? collectFXTargetTransform : transform;

        // Create object pool
        for(int i = 0; i < amountToPool; i++)
        {
            GameObject neuronCollect = Instantiate(collectFX, collectFXTargetTransform);
            neuronCollect.SetActive(false);
            pooledCollectFXs.Add(neuronCollect.GetComponent<ParticleSystem>());
        }
    }

    public void CollectNeuron(float mpAmount = 0)
    {
        playerMP.RechargeMagic(mpAmount);
        for(int i = 0; i < pooledCollectFXs.Count; i++)
        {
            if(!pooledCollectFXs[i].gameObject.activeInHierarchy)
            {
                pooledCollectFXs[i].gameObject.SetActive(true);
                pooledCollectFXs[i].Play();     // Note: Make sure the collect particle FX's StopAction setting is set to "Disabled" and not "Destroyed".
                return;
            }
        }
    }

}
