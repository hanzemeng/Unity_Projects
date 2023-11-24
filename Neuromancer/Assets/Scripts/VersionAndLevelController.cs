using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class VersionAndLevelController : MonoBehaviour
{
    [SerializeField] private TMP_Text versionAndLevelText;

    private void Update()
    {
        versionAndLevelText.text = $"BETA v. {Application.version} Level: {SceneManager.GetActiveScene().name}";
    }
}
