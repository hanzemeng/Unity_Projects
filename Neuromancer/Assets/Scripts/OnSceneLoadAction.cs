using UnityEngine;

public class OnSceneLoadAction : MonoBehaviour
{
    [SerializeField] private AudioManager.SoundResource startBGM;

    private void Start()
    {
        AudioManager.instance.PlayBackgroundMusic(startBGM);
    }
}
