using UnityEngine;
using UnityEngine.Events;

public class UnitMentalStamina : MonoBehaviour
{
    [SerializeField] private float mentalStamina, maxMentalStamina, mentalStaminaRegen;
    
    [System.NonSerialized] public UnityEvent<float> onStaminaDrainEvent = new UnityEvent<float>();
    [System.NonSerialized] public UnityEvent<float> onStaminaRechargeEvent = new UnityEvent<float>();
    [System.NonSerialized] public UnityEvent<bool> onZeroStaminaEvent = new UnityEvent<bool>();
    private Flashy flash;

    private void Awake() {
        mentalStamina = maxMentalStamina;
        if (!TryGetComponent<Flashy>(out flash)) {
            flash = gameObject.AddComponent<Flashy>();
        }
    }

    private void OnEnable() {
        onStaminaDrainEvent.AddListener(CheckZeroStamina);
    }

    private void OnDisable() {
        onStaminaDrainEvent.RemoveListener(CheckZeroStamina);
    }

    private void Update() {
        if(mentalStaminaRegen > 0) {
            RechargeMentalStamina(mentalStaminaRegen * Time.deltaTime);
        }
    }
    
    public void DrainMentalStamina(float drainAmount) {
        mentalStamina = Mathf.Clamp(mentalStamina - drainAmount, 0, maxMentalStamina);
        onStaminaDrainEvent.Invoke(mentalStamina);
        flash.Flash();
    }

    public void RechargeMentalStamina(float rechargeAmount) {
        mentalStamina = Mathf.Clamp(mentalStamina + rechargeAmount, 0, maxMentalStamina);
        onStaminaRechargeEvent.Invoke(mentalStamina);
    }

    private void CheckZeroStamina(float mentalStamina) {
        if (mentalStamina <= 0) {
            onZeroStaminaEvent.Invoke(true);
        }
    }
    
    public float GetMentalStamina() { return mentalStamina; }
    public float GetMaxMentalStamina() { return maxMentalStamina; }
}
