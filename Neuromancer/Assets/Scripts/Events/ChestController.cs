using UnityEngine;

public class ChestController : MonoBehaviour
{
    [SerializeField] private Animator myChest;
    private bool isOpen = false;
    public bool IsOpen { get {return isOpen; } }
    public void openChest()
    {
        myChest.Play("ChestOpen", 0, 0.0f);
        isOpen = true;
    }

}
