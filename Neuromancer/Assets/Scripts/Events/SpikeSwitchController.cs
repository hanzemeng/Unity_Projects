using System.Collections;
using UnityEngine;

// Used for any spike traps that are intended to be rails, settle for damage zones for now that deal damage at a fixed rate.
public class SpikeSwitchController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Animator spikeAnim;
    [SerializeField] private bool isDownAtStart = false;
    private bool isInInitialAnim = true;
    private bool isDown;
    private bool isOn;  // used for certain switches that will be activated on 

    private bool isDamaging;
    [SerializeField] private int playerDamage = 25;
    [SerializeField] private int unitDamage = 50;
    [SerializeField] private float damageFrequency = 1.5f;

    private void Start()
    {
        isOn = true;
        isDown = isDownAtStart;
        StartCoroutine(PlayInitialAnimation());
    }

    public void SpikeToggle()
    {
        if(isOn)
        {
            isDown = !isDown;
            string animationName = isDown ? "SpikeDown_Switch" : "SpikeUp_Switch";
            spikeAnim.Play(animationName, 0, 0.0f);
        }

    }

    public void StartDamage()
    {
        isDamaging = true;
        if(!isInInitialAnim)
        {
            AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.SPIKE_MOVEUP);
        }
        else
        {
            isInInitialAnim = false;
        }
        
    }

    public void EndDamage()
    {
        isDamaging = false;

        if(!isInInitialAnim)
        {
            AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.SPIKE_MOVEDOWN);
        }
        else
        {
            isInInitialAnim = false;
        }
    }

    private IEnumerator PlayInitialAnimation()
    {
        yield return new WaitForEndOfFrame();
        string animationName = isDown ? "SpikeDown_Switch" : "SpikeUp_Switch";
        spikeAnim.Play(animationName, 0, 0.0f);
    }


    private void OnTriggerEnter(Collider col)
    {
        if(isDamaging)
        {   
            if(col.gameObject.tag == Neuromancer.Unit.HERO_TAG || Neuromancer.Unit.IsAlly(col.transform) || Neuromancer.Unit.IsEnemy(col.transform))
            {
                Poison p = col.gameObject.AddComponent<Poison>();
                if(col.gameObject.tag == Neuromancer.Unit.HERO_TAG)
                {
                    p.Initiate(damageFrequency,playerDamage, 0);
                }
                else
                {
                    p.Initiate(damageFrequency, unitDamage, 0);
                }
            }
        } 
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == Neuromancer.Unit.HERO_TAG || Neuromancer.Unit.IsAlly(col.transform))
        {
            Poison c = col.gameObject.GetComponent<Poison>();
            if(c != null)
            {
                c.Remove();
            }
        }
    }

    // Will turn off all spike traps in the scene based on if the player hits a specific switch, to be used by events.
    public void DisableSpikes()
    {
        bool wasAlreadyDown = isDown;
        isDown = true;

        if(!wasAlreadyDown)
        {
            spikeAnim.Play("SpikeDown_Switch", 0, 0.0f);
        }

        isOn = false;
    }

}
