using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FrontDoor : ItemUse
{
    public GameObject barricade;

    public PlayerStress playerStress;
    public GameOver gameOver;

    void Start()
    {
        requiredItemName = ItemName.ITEM_AXE;
    }

    public override bool UseItem(Item item)
    {
        if(itemUsed)
        {
            StartCoroutine(Escape());
            return false;
        }

        if(null == item)
        {
            return false;
        }
        if(item.itemName == requiredItemName)
        {
            StartCoroutine(Use(item));
            itemUsed = true;
            return true;
        }
        else
        {
            Hint.hint.ShowHint("this item has no effect.");
            return false;
        }
    }

    IEnumerator Use(Item item)
    {
        GlobalVariable.TAKING_INPUT = false;
        PlayerTransition.playerTransition.HidePlayerUI();
        PostProcessing.postProcessing.FadeToBlack();
        yield return new WaitForSeconds(2f);
        item.PlayUseSound();
        yield return new WaitForSeconds(1.5f);
        item.PlayUseSound();
        yield return new WaitForSeconds(1.5f);
        item.PlayUseSound();
        yield return new WaitForSeconds(1.5f);
        Destroy(barricade);
        
        PostProcessing.postProcessing.FadeToWhite();
        yield return new WaitForSeconds(1f);
        GlobalVariable.TAKING_INPUT = true;
        PlayerTransition.playerTransition.ShowPlayerUI();
    }

    IEnumerator Escape()
    {
        GlobalVariable.TAKING_INPUT = false;

        PlayerTransition.playerTransition.HidePlayerUI();
        playerStress.SetStressLevel(0);
        Destroy(gameOver);

        BroadcastMessage("ObjectClicked");
        yield return new WaitForSeconds(0.25f);

        PostProcessing.postProcessing.FadeToBlack();
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("EscapeEnd");
    }
}
