using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MagicCircle : ItemUse
{
    public PlayerView playerView;
    public AudioSource touchSound;

    public PlayerStress playerStress;
    public GameOver gameOver;

    public override bool UseItem(Item item)
    {
        StartCoroutine(Escape());
        return false;
    }

    IEnumerator Escape()
    {
        GlobalVariable.TAKING_INPUT = false;

        PlayerTransition.playerTransition.HidePlayerUI();
        playerStress.SetStressLevel(0);
        Destroy(gameOver);

        touchSound.Play();
        playerView.FadeToWhite();
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("MagicCircleEnd");
    }
}
