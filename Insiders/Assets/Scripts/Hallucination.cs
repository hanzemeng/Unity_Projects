using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hallucination : MonoBehaviour
{
    public static Hallucination hallucination;
    void Awake()
    {
        hallucination = this;
    }

    public GameObject hallucinationView;
    public Image hallucinationImage;
    IEnumerator currentHallucination;

    public AudioSource hallucinationAudio;

    public void Start()
    {
        currentHallucination = C_Hallucinate(null);
    }

    public void Hallucinate(Sprite image)
    {
        StopCoroutine(currentHallucination);
        currentHallucination = C_Hallucinate(image);
        StartCoroutine(currentHallucination);
    }
    IEnumerator C_Hallucinate(Sprite image)
    {
        PlayerTransition.playerTransition.DisablePlayerCamera();
        PlayerTransition.playerTransition.HidePlayerUI();
        PostProcessing.postProcessing.StartDistortion();
        hallucinationImage.sprite = image;
        hallucinationView.SetActive(true);
        hallucinationAudio.Play();

        yield return new WaitForSeconds(0.2f);

        PlayerTransition.playerTransition.EnablePlayerCamera();
        PlayerTransition.playerTransition.ShowPlayerUI();
        PostProcessing.postProcessing.StopDistortion();
        hallucinationView.SetActive(false);
        hallucinationAudio.Stop();
    }
}
