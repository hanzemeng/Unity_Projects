using System.Collections;
using UnityEngine;

public class Duplicate : MonoBehaviour
{
    public float duplicateInterval;
    public float duplicateIntervalVariance;
    public Duplicator duplicator;
    private IEnumerator duplicationCoroutine;

    private void Start()
    {
        StartDuplication();
        GetComponent<UnitMentalStamina>().onZeroStaminaEvent.AddListener(OnStaminaIsZero);
    }

    private void StartDuplication()
    {
        if(null != duplicationCoroutine)
        {
            StopCoroutine(duplicationCoroutine);
        }
        duplicationCoroutine = DuplicationCoroutine();
        StartCoroutine(duplicationCoroutine);
    }

    private IEnumerator DuplicationCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(duplicateInterval + Random.Range(-1f*duplicateIntervalVariance, duplicateIntervalVariance));
            if(GetComponent<Neuromancer.NPCUnit>().isDead)
            {
                break;
            }
            duplicator.DuplicateAt(transform.position);
        }
    }

    private void OnStaminaIsZero(bool isZero)
    {
        if(null != duplicationCoroutine)
        {
            StopCoroutine(duplicationCoroutine);
            duplicationCoroutine = null;
        }
    }
}
