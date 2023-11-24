using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshLink))]
public class NavMeshLinkColliderGenerator : MonoBehaviour
{
    [Tooltip("The size of the generated box colliders")]
    [SerializeField] private Vector3 colliderSize = new Vector3(1,1,1);
    [Tooltip("The y Offset the collider will be above their start/end point of the navMeshLink")]
    [SerializeField] private float yOffset = 0.05f;

    private NavMeshLink currentLink;

    private void Awake()
    {
        currentLink = GetComponent<NavMeshLink>();
    }
    
    private void Start()
    {
        GenerateColliders();
    }

    private void GenerateColliders()
    {

        CreateBoxCollider(currentLink.startPoint);
        CreateBoxCollider(currentLink.endPoint);
    }

    private void CreateBoxCollider(Vector3 pos)
    {
        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.size = colliderSize;
        collider.center = pos + new Vector3(0,yOffset,0);
    }


}
