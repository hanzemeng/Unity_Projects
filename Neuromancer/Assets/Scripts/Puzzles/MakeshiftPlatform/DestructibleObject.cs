using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DestructibleObject : MonoBehaviour
{
    private Interactable interactable;
    private Breakable breakable;
    private Vector3 originalPosition;
    private bool isDestroyed = false;

    [Tooltip("The amount of times the object should be hit before being destroyed. Note that fodder units will subtract from health by 1.")]
    [SerializeField] private int objectHealth;  
    [SerializeField] private AnimationCurve shakeAnimCurve;
    [SerializeField] private AnimationCurve destructionShakeAnimCurve;
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeForce = 5f;
    [SerializeField] private AudioManager.SoundResource destroySFX = AudioManager.SoundResource.TREE_DESTROY;

    public UnityEvent<GameObject> InitiateDestroy;

    private void Awake()
    {
        breakable = GetComponent<Breakable>();
        interactable = GetComponent<Interactable>();
        originalPosition = transform.position;
    }


    // Uses the current unitAttacker, grabs a reference for both determining direction of particleFX AND for determining how much damage to inflict
    public void DamageObject(GameObject unitAttacker)
    {
        if(!isDestroyed)
        {
            objectHealth = Mathf.Clamp(objectHealth - 1, 0, objectHealth);
            if(objectHealth <= 0)
            {
                isDestroyed = true;
                StopAllCoroutines();
                transform.position = originalPosition;
                SetObjectToDestroy(unitAttacker);
                return;
            }
            StartCoroutine(ShakeObject(shakeDuration, shakeForce, shakeAnimCurve));
        }

        
    }

    private IEnumerator ShakeObject(float duration, float shakeStrength, AnimationCurve animCurve, bool isDestroy = false, GameObject unitAttacker = null)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        while(time < duration)
        {
            time += Time.deltaTime;
            float percent = Mathf.Clamp01(time / duration);
            float shakePercent = animCurve.Evaluate(percent);
            //Vector3 xOffset = transform.position + ShakeObject(shakePercent);
            Vector3 xOffset = Vector3.right * shakePercent * shakeStrength;
            transform.position = Vector3.Lerp(startPosition, startPosition + xOffset, shakePercent);
            yield return null;
        }

        if(isDestroy)
        {
            if(unitAttacker != null)
            {
                InitiateDestroy?.Invoke(unitAttacker);
                breakable.Break(false);
                InitiateDestroy.RemoveAllListeners();
            }
        }
    }

    public void SetObjectToDestroy(GameObject unitAttacker)
    {
        // Invoke event
        interactable.repeatInteraction = false;
        AudioManager.instance.PlayBackgroundSFX(destroySFX);
        StartCoroutine(ShakeObject(shakeDuration/1.5f, shakeForce * 1.2f, destructionShakeAnimCurve, true, unitAttacker));
        
    }
}
