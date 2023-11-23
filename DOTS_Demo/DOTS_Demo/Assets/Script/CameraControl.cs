using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float speedMultiplier;

    [SerializeField] private Vector3 moveLowerBound;
    [SerializeField] private Vector3 moveUpperBound;

    private Vector3 moveDirection;

    private void Update()
    {
        moveDirection = Vector3.zero;
        if(Input.GetKey(KeyCode.A))
        {
            moveDirection += Vector3.left;
        }
        if(Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector3.right;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += Time.deltaTime * speedMultiplier * moveSpeed * moveDirection;
        }
        else
        {
            transform.position += Time.deltaTime * moveSpeed * moveDirection;
        }

        if(transform.position.x < moveLowerBound.x || transform.position.y < moveLowerBound.y || transform.position.z < moveLowerBound.z)
        {
            transform.position = moveLowerBound;
        }
        else if(transform.position.x > moveUpperBound.x || transform.position.y > moveUpperBound.y || transform.position.z > moveUpperBound.z)
        {
            transform.position = moveUpperBound;
        }

    }
}
