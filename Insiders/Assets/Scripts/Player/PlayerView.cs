using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    public GameObject playerCameraObj;
    Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;
    float rotationX = 0;

    public Image fadeImage;

    float shakeAmount;
    bool cameraIsShaking;
    IEnumerator currentShakeCameraRoutine;

    void Start()
    {
        playerCamera = playerCameraObj.GetComponent<Camera>();
        cameraIsShaking = false;
        shakeAmount = 0.5f;
    }

    void Update()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, playerCamera.transform.localRotation.eulerAngles.z);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }
    public void FadeToRed()
    {
        StartCoroutine(C_Fade(new Color(1f,0f,0f,0f), new Color(1f,0f,0f,1f), 4f));
    }
    public void FadeToWhite()
    {
        StartCoroutine(C_Fade(new Color(1f,1f,1f,0f), new Color(1f,1f,1f,1f), 0.5f));
    }

    IEnumerator C_Fade(Color startColor, Color endColor, float speed)
    {
        float lerpAmount = 0f;
        while(lerpAmount<1f)
        {
            lerpAmount += speed*Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, endColor, lerpAmount);
            yield return null;
        }
    }

    Vector3 ShakeAmountToLocalPosition(float shakeAmount)
    {
        return new Vector3(0f, 1.775f+0.05f*shakeAmount, 0f);
    }
    Quaternion ShakeAmountToLocalRotation(float shakeAmount)
    {
        return Quaternion.Euler(rotationX, 0, -0.25f+0.5f*shakeAmount);
    }

    public void StartShakeCamera()
    {
        if(cameraIsShaking)
        {
            return;
        }
        cameraIsShaking = true;
        if(null != currentShakeCameraRoutine)
        {
            StopCoroutine(currentShakeCameraRoutine);
        }
        currentShakeCameraRoutine = C_StartShakeCamera();
        StartCoroutine(currentShakeCameraRoutine);
    }
    IEnumerator C_StartShakeCamera()
    {
        Transform playerCameraTransform = playerCameraObj.transform;
        while(true)
        {
            while(shakeAmount < 1f)
            {
                shakeAmount += 2f*Time.deltaTime;
                playerCameraTransform.localPosition = ShakeAmountToLocalPosition(shakeAmount);
                playerCameraTransform.localRotation = ShakeAmountToLocalRotation(shakeAmount);
                yield return null;
            }
            while(0f < shakeAmount)
            {
                shakeAmount -= 2f*Time.deltaTime;
                playerCameraTransform.localPosition = ShakeAmountToLocalPosition(shakeAmount);
                playerCameraTransform.localRotation = ShakeAmountToLocalRotation(shakeAmount);
                yield return null;
            }
        }
    }

    public void StopShakeCamera()
    {
        if(!cameraIsShaking)
        {
            return;
        }
        cameraIsShaking = false;
        if(null != currentShakeCameraRoutine)
        {
            StopCoroutine(currentShakeCameraRoutine);
        }
        currentShakeCameraRoutine = C_StopShakeCamera();
        StartCoroutine(currentShakeCameraRoutine);
    }
    IEnumerator C_StopShakeCamera()
    {
        Transform playerCameraTransform = playerCameraObj.transform;
        while(shakeAmount < 0.5f)
        {
            shakeAmount += 2f*Time.deltaTime;
            playerCameraTransform.localPosition = ShakeAmountToLocalPosition(shakeAmount);
            playerCameraTransform.localRotation = ShakeAmountToLocalRotation(shakeAmount);
            yield return null;
        }
        while(0.5f < shakeAmount)
        {
            shakeAmount -= 2f*Time.deltaTime;
            playerCameraTransform.localPosition = ShakeAmountToLocalPosition(shakeAmount);
            playerCameraTransform.localRotation = ShakeAmountToLocalRotation(shakeAmount);
            yield return null;
        }
        shakeAmount = 0.5f;
        playerCameraTransform.localPosition = ShakeAmountToLocalPosition(shakeAmount);
    }
}
