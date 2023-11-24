using UnityEngine;

public class NPCDialogue : PlayerInteractable {
    [SerializeField] private TextAsset dialogueText;

    // will be called in case we want NPCs to have a second dialogue when player talks to NPC already
    [SerializeField] private TextAsset followUpDialogueText;
    private bool hasTalked = false;

    protected override void Trigger()
    {
        if (!hasTalked || followUpDialogueText == null)
        {
            DialogueManager.dialogueManager.StartDialogue(dialogueText.name);  
            hasTalked = true;
        }
        else 
        {
            DialogueManager.dialogueManager.StartDialogue(followUpDialogueText.name);
        }
    }
}