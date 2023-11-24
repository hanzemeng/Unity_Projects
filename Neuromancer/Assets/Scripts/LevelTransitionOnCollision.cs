using UnityEngine;

public class LevelTransitionOnCollision : MonoBehaviour
{
    [SerializeField] private string transitionLevel; 
    [SerializeField] public int playerSpawnPointIndex = 0;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag(Neuromancer.Unit.HERO_TAG) && LevelManager.levelManager != null)
        {
            LevelManager.levelManager.LoadLevel(transitionLevel, playerSpawnPointIndex);
        }
    }
}
