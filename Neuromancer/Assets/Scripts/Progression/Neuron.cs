using System.Collections;
using UnityEngine;

public class Neuron : MonoBehaviour
{
    [SerializeField] private int skillPoint = 1;
    [SerializeField] private float rotateSpeed = 180;
    [SerializeField] private float playerMagicRegainAmount = 0.05f;    // How much MP the player will regain upon collect (keep a small amount)

    private bool isflying;

    private void Awake()
    {
        isflying = false;
    }

    private void Update()
    {
        transform.Rotate(0f, Time.deltaTime*rotateSpeed,0f, Space.Self);
    }

    private void OnTriggerEnter(Collider col)
    {
        FlyToTarget(col.transform);
    }

    public void FlyToTarget(Transform target)
    {
        if(isflying || target.gameObject.tag != Neuromancer.Unit.HERO_TAG) 
        {
            return;
        }
        isflying = true;
        StartCoroutine(FlyToTargetCoroutine(target.transform));
    }    

    private IEnumerator FlyToTargetCoroutine(Transform target)
    {
        float leapAmount = 0f;
        Vector3 startPosition = transform.position;

        // to prevent neurons from being collected at the same time
        float targetLeapAmount = Random.Range(0.75f, 1f);
        while(leapAmount<targetLeapAmount)
        {
            leapAmount += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, target.position + new Vector3(0,1.2f,0), leapAmount);
            yield return null;
        }
        PlayerProgression.playerProgression.skillPoint += skillPoint;
        PlayerProgression.playerProgression.onChangeEvent.Invoke();

        NeuronCollectManager.neuronCollector.CollectNeuron(playerMagicRegainAmount);

        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.NEURON_COLLECT);
        Destroy(gameObject);
    }
}
