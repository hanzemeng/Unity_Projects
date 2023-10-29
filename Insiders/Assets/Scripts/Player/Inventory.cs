using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Image[] backgroundUIs;
    public Image[] foregroundUIs;
    public Text descriptionUI;
    int selectedIndex;

    Color unselectBackground = new Color(255f/255f, 0/255f, 0/255f, 0f/255f);
    Color selectBackground = new Color(255f/255f, 0/255f, 0/255f, 255f/255f);
    Item[] items = new Item[8];

    public GameObject inventoryBar;
    IEnumerator currentInventoryBarRoutine;
    bool inventoryBarIsHidden;
    float hideInventoryBarTimer;

    void Start()
    {
        UpdateUI();
        HideInventoryBar();
    }

    void Update()
    {
        hideInventoryBarTimer -= Time.deltaTime;
        if(!inventoryBarIsHidden && hideInventoryBarTimer<0f)
        {
            HideInventoryBar();
        }

        if(!inventoryBarIsHidden)
        {
            if(null != items[selectedIndex])
            {
                if(ItemName.ITEM_WATCH == items[selectedIndex].itemName)
                {
                    int currentTime = (86220+(int)Timer.timer.GetElapsedGameTime())%86400;
                    int currentHour = currentTime/3600;
                    currentTime %= 3600;
                    int currentMinute = currentTime/60;
                    currentTime %= 60;
                    descriptionUI.text = "a watch. currently at " + currentHour.ToString() + ":" + currentMinute.ToString() + ":" + currentTime.ToString();
                }
                else
                {
                    descriptionUI.text = items[selectedIndex].description;
                }
            }
        }
    }

    public void SelectPreviousItem()
    {
        if(inventoryBarIsHidden)
        {
            ShowInventoryBar();
            return;
        }
        ShowInventoryBar();
        if(0 == selectedIndex)
        {
            selectedIndex = 7;
        }
        else
        {
            selectedIndex--;
        }
        UpdateUI();
    }
    public void SelectNextItem()
    {
        if(inventoryBarIsHidden)
        {
            ShowInventoryBar();
            return;
        }
        ShowInventoryBar();
        if(7 == selectedIndex)
        {
            selectedIndex = 0;
        }
        else
        {
            selectedIndex++;
        }
        UpdateUI();
    }

    public void PickUp(GameObject item)
    {
        ShowInventoryBar();
        item.transform.position = new Vector3(666f, 666f, 666f);
        for(int i=0; i<8; i++)
        {
            if(null == items[i])
            {
                items[i] = item.GetComponent<Item>();
                items[i].PlayPickUpSound();
                if(item.GetComponent<ItemTrigger>())
                {
                    item.GetComponent<ItemTrigger>().PickUpTrigger();
                }
                selectedIndex = i;
                UpdateUI();
                break;
            }
        }
    }
    public void DestroySelectedItem()
    {
        items[selectedIndex] = null;
        UpdateUI();
    }
    public Item GetSelectedItem()
    {
        if(inventoryBarIsHidden)
        {
            ShowInventoryBar();
            return null;
        }
        
        return items[selectedIndex];
    }
    void UpdateUI()
    {
        for(int i=0; i<8; i++)
        {
            if(i !=  selectedIndex)
            {
                backgroundUIs[i].color = unselectBackground;
            }
            else
            {
                backgroundUIs[i].color = selectBackground;
            }
            if(null != items[i])
            {
                foregroundUIs[i].sprite = items[i].UISprite;
                foregroundUIs[i].color = new Color(1f,1f,1f,1f);
            }
            else
            {
                foregroundUIs[i].sprite = null;
                foregroundUIs[i].color = new Color(0f,0f,0f,0f);
            }
        }
        if(null != items[selectedIndex])
        {
            if(ItemName.ITEM_WATCH == items[selectedIndex].itemName)
            {
                int currentTime = (86220+(int)Timer.timer.GetElapsedGameTime())%86400;
                int currentHour = currentTime/3600;
                currentTime %= 3600;
                int currentMinute = currentTime/60;
                currentTime %= 60;
                descriptionUI.text = "a watch. currently at " + currentHour.ToString() + ":" + currentMinute.ToString() + ":" + currentTime.ToString();
            }
            else
            {
                descriptionUI.text = items[selectedIndex].description;
            }
        }
        else
        {
            descriptionUI.text = "";
        }
    }

    public void ShowInventoryBar()
    {
        if(null != currentInventoryBarRoutine)
        {
            StopCoroutine(currentInventoryBarRoutine);
        }
        currentInventoryBarRoutine = C_ShowInventoryBar();
        StartCoroutine(currentInventoryBarRoutine);
        hideInventoryBarTimer = 3f;
        inventoryBarIsHidden = false;
    }
    IEnumerator C_ShowInventoryBar()
    {
        float currentY = inventoryBar.transform.localPosition.y;
        while(currentY<0f)
        {
            currentY += 800f*Time.deltaTime;
            inventoryBar.transform.localPosition = new Vector3(0f, currentY, 0f);
            yield return null;
        }
        inventoryBar.transform.localPosition = new Vector3(0f, 0f, 0f);
        if(null != items[selectedIndex])
        {
            if(ItemName.ITEM_WATCH == items[selectedIndex].itemName)
            {
                int currentTime = (86220+(int)Timer.timer.GetElapsedGameTime())%86400;
                int currentHour = currentTime/3600;
                currentTime %= 3600;
                int currentMinute = currentTime/60;
                currentTime %= 60;
                descriptionUI.text = "a watch. currently at " + currentHour.ToString() + ":" + currentMinute.ToString() + ":" + currentTime.ToString();
            }
            else
            {
                descriptionUI.text = items[selectedIndex].description;
            }
        }
    }

    public void HideInventoryBar()
    {
        if(null != currentInventoryBarRoutine)
        {
            StopCoroutine(currentInventoryBarRoutine);
        }
        currentInventoryBarRoutine = C_HideInventoryBar();
        StartCoroutine(currentInventoryBarRoutine);
        inventoryBarIsHidden = true;
        descriptionUI.text = "";
    }
    IEnumerator C_HideInventoryBar()
    {
        float currentY = inventoryBar.transform.localPosition.y;
        while(currentY>-100f)
        {
            currentY -= 800f*Time.deltaTime;
            inventoryBar.transform.localPosition = new Vector3(0f, currentY, 0f);
            yield return null;
        }
        inventoryBar.transform.localPosition = new Vector3(0f, -100f, 0f);
    }
}
