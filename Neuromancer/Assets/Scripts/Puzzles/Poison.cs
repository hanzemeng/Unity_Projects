using System.Collections;
using UnityEngine;
using EmeraldAI;
using EmeraldAI.Example;

public class Poison : MonoBehaviour
{
    private float interval = 3f; // Time interval in seconds
    private int damage = 5;
    private float grace_period = 12.5f;

    public void Initiate(float val, int dam, float grace)
    {
        interval = val;
        damage = dam;
        grace_period = grace;
    }

    private void Start()
    {
        if (gameObject.tag == Neuromancer.Unit.HERO_TAG)
        {
            PlayerController.player.GetComponent<EmeraldAIPlayerHealth>().DeathEvent.AddListener(Remove);
            StartCoroutine(DealDamageToHero());
        }
        else
        {
            transform.root.GetComponent<EmeraldAI.EmeraldAISystem>().DeathEvent.AddListener(Remove);
            StartCoroutine(DealDamageToAlly());
        }

    }

    private IEnumerator InitialGracePeriod()
    {
        Debug.Log("grace_period");
        yield return new WaitForSeconds(grace_period);
    }

    private IEnumerator DealDamageToHero()
    {
        Coroutine Grace = StartCoroutine(InitialGracePeriod());
        yield return Grace;
        while (true)
        {
            PlayerController.player.GetComponent<EmeraldAI.EmeraldAIPlayerDamage>().SendPlayerDamage(damage, gameObject.transform, null, false);
            Debug.Log("DEAL Damage to" + gameObject.name);
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator DealDamageToAlly()
    {
        Coroutine Grace = StartCoroutine(InitialGracePeriod());
        yield return Grace;
        while (true)
        {
            transform.root.GetComponent<EmeraldAI.EmeraldAISystem>().Damage(damage, EmeraldAISystem.TargetType.AI, gameObject.transform, 100);
            Debug.Log("DEAL Damage to" + gameObject.name);
            yield return new WaitForSeconds(interval);
        }
    }

    public void Remove()
    {
        if (gameObject.tag == Neuromancer.Unit.DEAD_TAG)
        {
            if(gameObject.GetComponent<PlayerController>() != null)
            {
                PlayerController.player.GetComponent<EmeraldAIPlayerHealth>().DeathEvent.RemoveListener(Remove);
            }
            else
            {
                transform.root.GetComponent<EmeraldAI.EmeraldAISystem>().DeathEvent.RemoveListener(Remove);
            }
        }

        Destroy(this);
    }
}
