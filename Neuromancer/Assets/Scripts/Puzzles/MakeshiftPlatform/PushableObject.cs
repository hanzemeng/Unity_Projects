using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class PushableObject : MonoBehaviour
{
    // [SerializeField] private int pushDistance;
    [Header("Object Push")]
    [Tooltip("Determines the minimum strength to be applied (amount will be multiplied based on unit)")]
    [SerializeField] private float pushStrength;
    [SerializeField] private AnimationCurve pushAnimCurve;
    [SerializeField] private float pushDuration = 1f;
    [SerializeField] private GameObject pushFX;
    [SerializeField] private Transform targetFXLocation;


    [Header("Object Shake")]
    [SerializeField] private AnimationCurve shakeAnimCurve;
    [SerializeField] private AnimationCurve shakeBeforeFallCurve;

    [Header("Surface Alignment")]
    [SerializeField] private bool applySurfaceAlignment = true;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private AnimationCurve surfaceAlignCurve;
    [SerializeField] private float surfaceAlignmentTimer = 0.25f;

    [Header("Fall Parameters")]
    [SerializeField] private float rotationForceAmountBeforeFall = 5f;
    [Range(0f, 10f), SerializeField] private float yOffsetBeforeFall = 0f;
    [SerializeField] private float rotationTime = 2f;
    [SerializeField] private AnimationCurve fallAnimCurve;

    [Header("Non-Deterministic Fall Parameters")]
    [SerializeField] private bool isUsingRigidbody = true;
    [SerializeField] private float fallRotationTarget= 180f;
    [SerializeField] private float fallDuration = 5f;
    [SerializeField] private AnimationCurve mainFallAnimCurve;

    [Header("Destruction Parameters")]
    [SerializeField] private float matchGameObjectDuration = 0.05f;

    // Will be used to determine if the gameObject has reached the edge or not
    private Interactable interactable;
    private Breakable breakable;  // To be applied
    private Rigidbody rb;
    private bool isFalling = false;
    private bool hasReachedEdge = false;

    private void Awake()
    {   
        interactable = GetComponent<Interactable>();
        breakable = GetComponent<Breakable>();
        rb = GetComponent<Rigidbody>();
    }

    public void PushObject(GameObject unit)
    {
        if(unit != null && !hasReachedEdge)
        {
            _ = pushFX != null ? Instantiate(pushFX, targetFXLocation.transform.position, Quaternion.identity) : null;
            Vector3 targetPosition = transform.position + (transform.forward * pushStrength);
            StartCoroutine(LerpPush(targetPosition, pushDuration));
        }
    }

    // Lerp to next position
    private IEnumerator LerpPush(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        while(time < duration)
        {
            time += Time.deltaTime;
            float percent = Mathf.Clamp01(time / duration);
            float shakePercent = shakeAnimCurve.Evaluate(percent);
            //Vector3 xOffset = transform.position + ShakeObject(shakePercent);
            Vector3 xOffset = Vector3.right * shakePercent;
            float curvePercent = pushAnimCurve.Evaluate(percent);
            transform.position = Vector3.Lerp(startPosition, targetPosition + xOffset, curvePercent);
            
            if(applySurfaceAlignment) { SurfaceAlignment(); } 

            yield return null;
        }
        
        //transform.position = new Vector3(targetPosition.x, currentYOffset, targetPosition.z);
        transform.position = targetPosition;
        if(applySurfaceAlignment) { SurfaceAlignment(); } 
    }

    private void SurfaceAlignment()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit info = new RaycastHit();
        Quaternion rotationRef = Quaternion.Euler(0,0,0);

        if(Physics.Raycast(ray, out info, groundMask))
        {
            rotationRef = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, info.normal), surfaceAlignCurve.Evaluate(surfaceAlignmentTimer));
            transform.rotation = Quaternion.Euler(rotationRef.eulerAngles.x, transform.eulerAngles.y, rotationRef.eulerAngles.z);
            // currentYOffset = transform.position.y - (transform.position.y - info.point.y);
        }
    }

    public void InitiateFall()
    {
        if(!hasReachedEdge)
        {
            StopAllCoroutines();
            interactable.repeatInteraction = false;
            hasReachedEdge = true;
            StartCoroutine(InitiateFall(rotationTime, rotationForceAmountBeforeFall, fallAnimCurve));     
        }

    }

    // Will do two (or three depending on if rigidbody is being used) things:
    //  1.) Deactives the kinematic property and reactives gravity of the rigidbody
    //  2.) Lerps forward rotation of gameObject forward 

    private IEnumerator InitiateFall(float duration, float rotationForce, AnimationCurve animCurve, bool shakeObject = true)
    {
        Debug.Log("Statue fall has begun");
        float time = 0;
        transform.position = transform.position + (Vector3.up * yOffsetBeforeFall);
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(Vector3.right * rotationForce);

        while(time < duration)
        {
            time += Time.deltaTime;
            float percent = Mathf.Clamp01(time / duration);

            // Initiate tilt before properly fall
            float fallPercent = animCurve.Evaluate(percent);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, fallPercent);

            if(shakeObject)
            {   
                // Shake gameObject before falling
                float shakePercent = shakeBeforeFallCurve.Evaluate(percent);
                Vector3 xOffset = Vector3.right * shakePercent;
                transform.position = Vector3.Lerp(transform.position, transform.position + xOffset, shakePercent);
            }

            yield return null;
        }
        
        if(isUsingRigidbody)
        {
            // Will ignore all collisions with the player or units.
            //Physics.IgnoreLayerCollision(6, 8);
            gameObject.layer = LayerMask.NameToLayer("NoCollide");
            transform.position = transform.position + (Vector3.up * yOffsetBeforeFall);
            rb.isKinematic = false;
            //rb.constraints = RigidbodyConstraints.FreezePositionZ;
            rb.useGravity = true;
        }
        else
        {
            if(!isFalling)
            {
                StartCoroutine(InitiateFall(fallDuration, fallRotationTarget, mainFallAnimCurve, shakeObject: false));
            }

        }   
        isFalling = true;        
    }

    public void DestroyAndReplacePushable(MakeshiftPlatformController target)
    {
        StopAllCoroutines();
        StartCoroutine(MatchTargetObjectTransform(target, matchGameObjectDuration));
    }

    // Has gameObject rotates and moves towards another gameObject to match what it looks like so it doesn't appear as if the gameObject immediately snaps into place.
    private IEnumerator MatchTargetObjectTransform(MakeshiftPlatformController target, float duration)
    {
        float time = 0;
       //
        while(time < duration)
        {
            time += Time.deltaTime;
            float percent = Mathf.Clamp01(time / duration);

            transform.rotation = Quaternion.Lerp(transform.rotation, target.MainPlatform.transform.rotation, percent);
            transform.position = Vector3.Lerp(transform.position, target.MainPlatform.transform.position, percent);
            yield return null;
        }
        
        transform.rotation = target.MainPlatform.transform.rotation;
        transform.position = target.MainPlatform.transform.position;
        AudioManager.instance.PlayMusicAtVector(AudioManager.SoundResource.INTERACTABLE_SUCCESS, transform.position);
        breakable.Break();
        target.ActivatePlatform();
    }

    
}
