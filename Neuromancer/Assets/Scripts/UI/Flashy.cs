using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashy : MonoBehaviour
{
    [Header("On Damage Flash Settings")]
    private Shader flashShader;
    private List<Material> mats = new List<Material>();
    private List<Shader> shaders = new List<Shader>();
    [Tooltip("Will not flash twice within interval seconds")]
    [SerializeField] private float flashInterval = 0.3f;
    [SerializeField] private float flashDuration = 0.05f;
    private float flashTimer;
    private bool canFlash = true;

    private void Awake() {
        flashShader = Shader.Find("Custom/FlashShader");
        Renderer[] rends = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (Renderer r in rends) {
            mats.Add(r.material);
            shaders.Add(r.material.shader);
        }
        flashTimer = flashInterval;
    }

    private void Update() {
        if (!canFlash) { flashTimer -= Time.deltaTime; }
        if (flashTimer <= 0f) {
            canFlash = true;
            flashTimer = flashInterval;
        }
    }
    
    public void Flash(Color? color = null) {
        if (!canFlash) { return; }
        for (int i = 0; i < mats.Count; i++)
            StartCoroutine(FlashRoutine(i, color));
        canFlash = false;
    }

    private IEnumerator FlashRoutine(int index, Color? color = null) {
        // Flash white
        mats[index].shader = flashShader;
        mats[index].SetColor("_ColorChange", Color.white);

        // If there is additional coloring, also flash color
        if (color != null) {
            yield return new WaitForSeconds(flashDuration / 2f);
            mats[index].SetColor("_ColorChange", (Color)color);
        }

        yield return new WaitForSeconds(flashDuration);
        mats[index].shader = shaders[index];
    }
}