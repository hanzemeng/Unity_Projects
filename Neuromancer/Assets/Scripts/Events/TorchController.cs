using UnityEngine;

public class TorchController : MonoBehaviour
{
    public void ActivateTorch()
    {
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.FIRE_LIT);
    }

    public void ExtinguishTorch()
    {
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.FIRE_EXTINGUISH);
    }
}
