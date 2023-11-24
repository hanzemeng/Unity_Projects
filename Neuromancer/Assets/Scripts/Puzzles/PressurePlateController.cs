using UnityEngine;

public class PressurePlateController : MonoBehaviour
{
    private Animator anim;
    private bool isPressed = false;
    [SerializeField] private ParticleSystem puzzleSolvedFX;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnPlatePressed()
    {
        isPressed = true;
        anim.SetBool("IsPressed", isPressed);
    }

    public void OnPlateExit()
    {
        isPressed = false;
        anim.SetBool("IsPressed", isPressed);
    }

    public void PlayPressSound()
    {
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.PRESSURE_PLATE_PRESSED);
    }
    public void PlayExitSound()
    {
        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.PRESSURE_PLATE_RELEASED);
    }

    public void OnPuzzleSolvedSuccess()
    {
        if(puzzleSolvedFX != null) { puzzleSolvedFX.Play(); }
        switch(isPressed)
        {
            case true:
            default:
                anim.Play("Puzzle Solved", 0, 0.0f);
                break;
            case false:
                anim.Play("Puzzle Solved Ver2", 0, 0.0f);
                break;
        }
    }

}
