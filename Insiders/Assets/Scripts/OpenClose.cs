using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenClose : MonoBehaviour
{
    public int openValue;

    private Animator myAnimator;
    private Animator additionalAnimator;
    public bool objectOpen;
    public bool objectOpenAdditional;
    public GameObject animateAdditional;
    private bool hasAdditional = false;
    float myNormalizedTime;

    private AudioSource audioSource;

    void Start()
    {
        myAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if(myAnimator == null)
        {
            myAnimator = GetComponentInParent<Animator>();
        }
        
        if(objectOpen == true)
        {
            myAnimator.Play("Open", 0, 1.0f);
            GlobalVariable.OPEN_VALUE += openValue;
        }
        if(animateAdditional != null)
        {
            if (animateAdditional.GetComponent<OpenClose>())
            {
                additionalAnimator = animateAdditional.GetComponent<Animator>();
                hasAdditional = true;
                objectOpenAdditional = animateAdditional.GetComponent<OpenClose>().objectOpen;
            }
        } 
        else
        {
            hasAdditional = false;
        }
    }

    public void ObjectClicked()
    {
        myNormalizedTime = myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if(hasAdditional == false && myNormalizedTime >= 1.0)
        {
            if(objectOpen == true)
            {
                myAnimator.Play("Close", 0, 0.0f);
                objectOpen = false;
                GlobalVariable.OPEN_VALUE -= openValue;
                audioSource.Play();
            }
            else
            {
                myAnimator.Play("Open", 0, 0.0f);
                objectOpen = true;
                GlobalVariable.OPEN_VALUE += openValue;
                audioSource.Play();
            }
        }

        if(hasAdditional == true && myNormalizedTime >= 1.0)
        {
            if (objectOpen == true)
            {
                myAnimator.Play("Close", 0, 0.0f);
                objectOpen = false;
                GlobalVariable.OPEN_VALUE -= openValue;
                audioSource.Play();
                animateAdditional.GetComponent<OpenClose>().objectOpenAdditional = false;

                if (objectOpenAdditional == true)
                {
                    additionalAnimator.Play("Close", 0, 0.0f);
                    objectOpenAdditional = false;
                    animateAdditional.GetComponent<OpenClose>().objectOpen = false;
                }
            }
            else
            {
                myAnimator.Play("Open", 0, 0.0f);
                objectOpen = true;
                GlobalVariable.OPEN_VALUE += openValue;
                audioSource.Play();
                animateAdditional.GetComponent<OpenClose>().objectOpenAdditional = true;

                if (objectOpenAdditional == false)
                {
                    additionalAnimator.Play("Open", 0, 0.0f);
                    objectOpenAdditional = true;
                    animateAdditional.GetComponent<OpenClose>().objectOpen = true;

                }
            }
        }
    }
}
