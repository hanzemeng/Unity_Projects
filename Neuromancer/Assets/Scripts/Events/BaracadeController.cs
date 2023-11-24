using UnityEngine;

public class BaracadeController : MonoBehaviour
{
    [SerializeField] private Animator myBaracade;
    public void OpenBaracade()
    {
        myBaracade.Play("BaracadeOpen", 0, 0.0f);
    }

    public void OpenBaracadeV2()
    {
        myBaracade.Play("BaracadeOpenV2", 0, 0.0f);
    }

    public void CloseBaracadeV2()
    {
        myBaracade.Play("BaracadeCloseV2", 0, 0.0f);
    }

    public void CloseBaracade()
    {
        myBaracade.Play("BaracadeClose", 0, 0.0f);
    }

    public void PlayOpenSound()
    {
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.SPIKE_MOVEDOWN);
    }
    public void PlayCloseSound()
    {
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.SPIKE_MOVEUP);
    }
}
