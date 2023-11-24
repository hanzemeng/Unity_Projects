using UnityEngine;

public class TabButton : MonoBehaviour {

    [SerializeField] private GameObject tabFocus;
    [SerializeField] private GameObject menu;

    public GameObject GetTabFocus() {
        return tabFocus;
    }

    public GameObject GetMenu() {
        return menu;
    }

    public void Click() {
        PauseHandler.current.SwitchMenu(this);
    }
}
