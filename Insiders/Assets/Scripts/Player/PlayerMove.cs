using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerMove : MonoBehaviour
{
    public float walkSpeed = 1.15f;
    public float runSpeed = 4.0f;
    public float gravity = 1500.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    Vector3 previousPosition;

    public PlayerView playerView;
    public PlayerSound playerSound;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        previousPosition = transform.position;
    }

    void Update()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        //bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isRunning = false;
        float curSpeedX = (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical");
        float curSpeedY = (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal");
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        Vector3 currentPosition = transform.position;
        float speed = (currentPosition-previousPosition).magnitude / Time.deltaTime;
        previousPosition = currentPosition;

        if(speed>1f)
        {
            playerSound.PlayWalkSound();
            playerView.StartShakeCamera();
        }
        else
        {
            playerSound.StopWalkSound();
            playerView.StopShakeCamera();
        }
    }
}