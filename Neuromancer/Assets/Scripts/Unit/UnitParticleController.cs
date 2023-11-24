using UnityEngine;

public class UnitParticleController : MonoBehaviour
{
    // Currently very barebones, but I can imagine that this may get expanded upon to play particle effects for other events like possessing a unit, removing from party, etc.

    [Header("Aura Particle Effect")]
    [SerializeField] private GameObject loopingAuraFX;
    [SerializeField] private Transform loopingAuraTargetTransform;
    [SerializeField] private bool rotate90DegreesOnX = false;
    private ParticleSystem currentLoopingAuraFX;

    private void Start()
    {   
        currentLoopingAuraFX = Instantiate(loopingAuraFX, loopingAuraTargetTransform).GetComponent<ParticleSystem>();
        currentLoopingAuraFX.transform.rotation = rotate90DegreesOnX ? Quaternion.Euler(-90,0,0) : Quaternion.identity;
    }

    public void PauseAuraFX()
    {
        currentLoopingAuraFX.Stop();
    }

    public void PlayAuraFX()
    {
        currentLoopingAuraFX.Play();
    }
}
