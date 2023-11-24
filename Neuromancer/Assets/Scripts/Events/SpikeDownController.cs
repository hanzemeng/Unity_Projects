using System.Collections;
using UnityEngine;

public class SpikeDownController : MonoBehaviour
{
    [SerializeField] private Animator mySpike;
    [SerializeField] private Vector3 moveDownPositioin;
    [SerializeField] private float moveTime;
    private bool isDown;

    public void spikeDown()
    {
        mySpike.Play("SpikeDown", 0, 0.0f);
    }

    private void Start()
    {
        isDown = false;
    }
    public void SpikeMoveDown()
    {
        if(isDown)
        {
            return;
        }
        isDown = true;

        StartCoroutine(SpikeMoveDownCoroutine());
    }
    private IEnumerator SpikeMoveDownCoroutine()
    {
        Vector3 originalPosition = transform.position;
        float lerpAmount = 0f;
        while(lerpAmount<1f)
        {
            lerpAmount += Time.deltaTime / moveTime;
            transform.position = Vector3.Lerp(originalPosition, moveDownPositioin, lerpAmount);
            yield return null;
        }
        transform.position = moveDownPositioin;
    }

    public void PlaySpikeMoveDownSFX()
    {
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.SPIKE_MOVEDOWN);
    }
}
