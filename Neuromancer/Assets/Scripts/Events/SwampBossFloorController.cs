using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwampBossFloorController : MonoBehaviour
{
    [SerializeField] private GameObject nextFloorGameObject;
    [SerializeField] private GameObject nextFloorEffect;
    [SerializeField] private GameObject smokeEffectPrefab;
    [SerializeField] private List<GameObject> spawnersGameObjects;
    private bool isShowing;

    private void Awake()
    {
        isShowing = false;
    }

    public void ShowNextFloor()
    {
        if(isShowing)
        {
            return;
        }
        isShowing = true;
        StartCoroutine(ShowNextFloorCoroutine());
    }

    private IEnumerator ShowNextFloorCoroutine()
    {
        foreach(GameObject spawnerGameObject in spawnersGameObjects)
        {
            spawnerGameObject.SetActive(false);
        }
        Instantiate(smokeEffectPrefab, nextFloorGameObject.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        nextFloorGameObject.SetActive(true);
        nextFloorEffect.SetActive(true);
    }
}
