using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NavMeshSurfaceGenerator : MonoBehaviour
{
    public static NavMeshSurfaceGenerator current;
    public bool buildOnStart = false;

    private NavMeshSurface[] surfaces;
    public NavMeshSurface[] Surfaces { get { return surfaces; } }   // Added for navmesh visualizer

    private void Awake()
    {
        if (current == null)
        {
            current = this;
            surfaces = GetComponents<NavMeshSurface>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (buildOnStart)
        {
            StartCoroutine(BuildNavMeshDelay(0.2f));
        }
    }

    private IEnumerator BuildNavMeshDelay(float delay) {
        yield return new WaitForSeconds(delay);
        foreach (NavMeshSurface sur in surfaces)
        {
            sur.BuildNavMesh();
        }
    }

    public void UpdateNavMesh(float delay)
    {
        StartCoroutine(UpdateNavMeshCoroutine(delay));
    }

    private IEnumerator UpdateNavMeshCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableAllUnits(false);
        foreach (NavMeshSurface sur in surfaces)
        {
            sur.UpdateNavMesh(sur.navMeshData);
        } 
        EnableAllUnits(true);
    }

        // Used to toggle all or none of the units based on the inputted boolean
    private void EnableAllUnits(bool isEnabled)
    {
        // Find all units and disables their EmeraldAI component
        Neuromancer.NPCUnit[] allUnits = FindObjectsByType<Neuromancer.NPCUnit>(FindObjectsSortMode.None);
        if(allUnits.Length > 0)
        {
            // Just so we dont see a lot of warning pop ups occuring.
            foreach (Neuromancer.NPCUnit unit in allUnits) 
            {
                unit.GetComponent<NavMeshAgent>().enabled = isEnabled;
                unit.EmeraldComponent.enabled = isEnabled;
            }
        }
    }
}
