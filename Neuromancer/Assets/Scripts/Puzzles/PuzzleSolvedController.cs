using UnityEngine;

public class PuzzleSolvedController : MonoBehaviour
{
    [SerializeField] AudioManager.SoundResource puzzleSolvedSound = AudioManager.SoundResource.PUZZLE_SOLVED;
    public void PlayPuzzleSolvedSound()
    {
        AudioManager.instance.PlayBackgroundSFX(puzzleSolvedSound);
    }

}
