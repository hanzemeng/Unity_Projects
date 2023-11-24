using UnityEngine;
using UnityEngine.UI;
using Neuromancer;

public class UnitIconUI : MonoBehaviour {

    [SerializeField] private Image image;
    [SerializeField] private UnitHealthBarController healthBarController;
    [SerializeField] private Sprite noEnemyIcon;

    public void SetUnit(NPCUnit unit) {
        healthBarController.SetUnit(unit);
        if (unit != null) {
            image.sprite = unit.GetIcon() != null ? unit.GetIcon() : noEnemyIcon;
        }
        
    }
}