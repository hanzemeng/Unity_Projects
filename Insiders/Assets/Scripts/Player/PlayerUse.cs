using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUse : MonoBehaviour
{
    public GameObject mainCamera;
    public Crosshair crosshair;

    private Inventory inventory;

    void Start()
    {
        inventory = GetComponent<Inventory>();
        UpdateInteractionOption();
    }

    void Update()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        UpdateInteractionOption();
        if(Input.GetMouseButtonDown(0))
        {
            RaycastCheck();
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            inventory.SelectPreviousItem();
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            inventory.SelectNextItem();
        }
    }
    
    void UpdateInteractionOption()
    {
        RaycastHit hit;

        if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hit, 2.3f))
        {
            Puzzle puzzleObj = hit.collider.gameObject.GetComponent<Puzzle>();
            Item item = hit.collider.gameObject.GetComponent<Item>();
            ItemUse itemUse = hit.collider.gameObject.GetComponent<ItemUse>();
            Hide hide = hit.collider.gameObject.GetComponent<Hide>();
            OpenClose regularObj = hit.collider.gameObject.GetComponent<OpenClose>();
            if(puzzleObj)
            {
                crosshair.SetInteractionOptionText(puzzleObj.GetDisplayMessage());
            }
            else if(item)
            {
                crosshair.SetInteractionOptionText("collect");
            }
            else if(itemUse)
            {
                crosshair.SetInteractionOptionText(itemUse.GetDisplayMessage());
            }
            else if(hide)
            {
                crosshair.SetInteractionOptionText("hide");
            }
            else if(regularObj)
            {
                if(regularObj.objectOpen)
                {
                    crosshair.SetInteractionOptionText("close");
                }
                else
                {
                    crosshair.SetInteractionOptionText("open");
                }
            }
            else
            {
                crosshair.SetInteractionOptionText("");
            }
        }
        else
        {
            crosshair.SetInteractionOptionText("");
        }
    }

    void RaycastCheck()
    {
        RaycastHit hit;

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hit, 2.3f))
        {
            if(hit.collider.gameObject.GetComponent<Puzzle>())
            {
                hit.collider.gameObject.GetComponent<Puzzle>().LoadPuzzle();
                Hint.hint.ShowTutorial(Hint.TutorialMessage.T_PUZZLE);
            }
            else if(hit.collider.gameObject.GetComponent<Item>())
            {
                inventory.PickUp(hit.collider.gameObject);
                Hint.hint.ShowTutorial(Hint.TutorialMessage.T_ITEM);
            }
            else if(hit.collider.gameObject.GetComponent<ItemUse>())
            {
                if(hit.collider.gameObject.GetComponent<ItemUse>().UseItem(inventory.GetSelectedItem()))
                {
                    inventory.DestroySelectedItem();
                }
            }
            else if(hit.collider.gameObject.GetComponent<Hide>())
            {
                hit.collider.gameObject.GetComponent<Hide>().LoadHide();
            }
            else if(hit.collider.gameObject.GetComponent<OpenClose>())
            {
                hit.collider.gameObject.BroadcastMessage("ObjectClicked");
            }
        }
    }
}
