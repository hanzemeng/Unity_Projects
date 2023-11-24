using UnityEngine;

public class LevelTransition : PlayerInteractable
{
    
    [SerializeField] public string transitionLevel;
    [SerializeField] public int playerSpawnPointIndex;
    [SerializeField] private AudioManager.SoundResource transitionSFX = AudioManager.SoundResource.TO_NEXT_SCENE_DEFAULT;

    protected override void Trigger()
    {
        LevelManager.levelManager.LoadLevel(transitionLevel, playerSpawnPointIndex);
        AudioManager.instance.PlayBackgroundSFX(transitionSFX);
    }
}
