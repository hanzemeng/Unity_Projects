using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUserInput : MonoBehaviour
{
    public static GameUserInput gameUserInput;

    public MouseInput swipeArea;
    public MouseInput facebook;
    public MouseInput twitter;
    public MouseInput settings;
    public MouseInput alert;

    private void Awake()
    {
        if(null != gameUserInput)
        {
            Destroy(gameObject);
            return;
        }
        gameUserInput = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ListenNodesInput()
    {
        List<GameObject> nodes = GameControl.gameControl.nodes;
        if(null == nodes)
        {
            return;
        }
        for(int i=0; i<nodes.Count; i++)
        {
            int index = i;
            nodes[i].GetComponent<MouseInput>().onClickEvent.AddListener(() => GameControl.gameControl.OnNodeClick(index));
            nodes[i].GetComponent<Collider>().enabled = true;
        }
    }

    public void StopListenNodesInput()
    {
        List<GameObject> nodes = GameControl.gameControl.nodes;
        if(null == nodes)
        {
            return;
        }
        for(int i=0; i<nodes.Count; i++)
        {
            nodes[i].GetComponent<MouseInput>().onClickEvent.RemoveAllListeners();
            nodes[i].GetComponent<Collider>().enabled = false;
        }
    }

    public void ListenNewLevelSlider()
    {
        swipeArea.onSwipeLeftEvent.AddListener(() => GameControl.gameControl.OnSwipeLeft());
        swipeArea.onSwipeRightEvent.AddListener(() => GameControl.gameControl.OnSwipeRight());
        swipeArea.transform.GetComponent<Collider>().enabled = true;
    }
    public void StopListenNewLevelSlider()
    {
        swipeArea.onSwipeLeftEvent.RemoveAllListeners();
        swipeArea.onSwipeRightEvent.RemoveAllListeners();
        swipeArea.transform.GetComponent<Collider>().enabled = false;
    }

    public void ListenFacebookInput()
    {
        facebook.onClickEvent.AddListener(() => GameControl.gameControl.OnFacebookClick());
        facebook.transform.GetComponent<Collider>().enabled = true;
    }
    public void StopListenFacebookInput()
    {
        facebook.onClickEvent.RemoveAllListeners();
        facebook.transform.GetComponent<Collider>().enabled = false;
    }

    public void ListenTwitterInput()
    {
        twitter.onClickEvent.AddListener(() => GameControl.gameControl.OnTwitterClick());
        twitter.transform.GetComponent<Collider>().enabled = true;
    }
    public void StopListenTwitterInput()
    {
        twitter.onClickEvent.RemoveAllListeners();
        twitter.transform.GetComponent<Collider>().enabled = false;
    }

    public void ListenSettingsInput()
    {
        settings.onClickEvent.AddListener(() => GameControl.gameControl.OnSettingsClick());
        settings.transform.GetComponent<Collider>().enabled = true;
    }
    public void StopListenSettingsInput()
    {
        settings.onClickEvent.RemoveAllListeners();
        settings.transform.GetComponent<Collider>().enabled = false;
    }

    public void ListenAlertInput()
    {
        alert.onClickEvent.AddListener(() => GameControl.gameControl.OnAlertClick());
        alert.transform.GetComponent<Collider>().enabled = true;
    }
    public void StopListenAlertInput()
    {
        alert.onClickEvent.RemoveAllListeners();
        alert.transform.GetComponent<Collider>().enabled = false;
    }
}
