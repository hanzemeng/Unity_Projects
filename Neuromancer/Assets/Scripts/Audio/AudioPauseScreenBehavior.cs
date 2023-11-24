using UnityEngine;
public class AudioPauseScreenBehavior : MonoBehaviour
{

    private AudioSource audiosource;
    private bool wasPlaying = false;

    //subscribe to pauseHandlerAction
    private void Start()
    {
        PauseHandler.onPauseEvent.AddListener(PauseMusicPause);
        PauseHandler.onResumeEvent.AddListener(ResumeMusicResume); 
        audiosource = GetComponent<AudioSource>();
        if(audiosource == null)
        {
            Destroy(this);
        }
    }
    //MusicOnPause Behavior
    private void PauseMusicPause(bool isMainMenu)
    {
       
        if (isMainMenu && (wasPlaying = audiosource.isPlaying))
            audiosource.Pause();
        
       
    }
    //MusicOnResume Behavior
    private void ResumeMusicResume()
    {
        if(wasPlaying)
            audiosource.UnPause();
    }

    private void OnDestroy()
    {
        PauseHandler.onPauseEvent.RemoveListener(PauseMusicPause);
        PauseHandler.onResumeEvent.RemoveListener(ResumeMusicResume);

    }
}
