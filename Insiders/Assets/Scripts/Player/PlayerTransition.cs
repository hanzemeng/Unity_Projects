using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransition : MonoBehaviour
{
    CharacterController characterController;
    PlayerMove playerMove;
    PlayerUse playerUse;
    PlayerView playerView;
    public PlayerSound playerSound;
    Inventory inventory;
    Crosshair crosshair;

    public static PlayerTransition playerTransition;
    void Awake()
    {
        playerTransition = this;
        characterController = GetComponent<CharacterController>();
        playerMove = GetComponent<PlayerMove>();
        playerUse = GetComponent<PlayerUse>();
        playerView = GetComponent<PlayerView>();
        inventory = GetComponent<Inventory>();
        crosshair = GetComponent<Crosshair>();
    }
    

    public void ShowPlayerUI()
    {
        characterController.enabled = true;
        playerMove.enabled = true;
        playerUse.enabled = true;
        playerView.enabled = true;
        inventory.enabled = true;
        crosshair.SetCrosshairImageColor(new Color(1f,1f,1f,1f));
    }
    public void HidePlayerUI()
    {
        characterController.enabled = false;
        playerMove.enabled = false;
        playerUse.enabled = false;
        playerView.StopShakeCamera();
        playerView.enabled = false;
        playerSound.StopWalkSound();
        inventory.HideInventoryBar();
        crosshair.SetCrosshairImageColor(new Color(0f,0f,0f,0f));
        crosshair.SetInteractionOptionText("");
    }
    public void EnablePlayerCamera()
    {
        playerView.playerCameraObj.SetActive(true);
    }
    public void DisablePlayerCamera()
    {
        playerView.playerCameraObj.SetActive(false);
    }
}
