using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : ItemUse
{
    public AudioSource note;
    public bool isPlaying;

    void Start()
    {
        isPlaying = false;
        requiredItemName = ItemName.ITEM_BATTERY;
    }

    public override bool UseItem(Item item)
    {
        if(itemUsed)
        {
            if(!isPlaying)
            {
                StartCoroutine(PlayNotes());
            }
            return false;
        }

        if(null == item)
        {
            return false;
        }
        if(item.itemName == requiredItemName)
        {
            item.PlayUseSound();
            itemUsed = true;
            return true;
        }
        else
        {
            Hint.hint.ShowHint("this item has no effect.");
            return false;
        }
    }

    IEnumerator PlayNotes()
    {
        isPlaying = true;

        note.pitch = (float) 2 * 0.1f + 0.5f;
        note.Play();
        yield return new WaitForSeconds(0.6f);
        note.pitch = (float) 6 * 0.1f + 0.5f;
        note.Play();
        yield return new WaitForSeconds(0.6f);
        note.pitch = (float) 3 * 0.1f + 0.5f;
        note.Play();
        yield return new WaitForSeconds(0.6f);
        note.pitch = (float) 5 * 0.1f + 0.5f;
        note.Play();
        yield return new WaitForSeconds(0.6f);
        note.pitch = (float) 6 * 0.1f + 0.5f;
        note.Play();
        yield return new WaitForSeconds(0.6f);
        isPlaying = false;
    }
}
