using UnityEngine;

public class FieldTutorialHandler : MonoBehaviour {
    private void OnDisable() {
        DialogueManager.dialogueManager.onDialogueFinish.RemoveListener(TransitionScene);
    }

    public void FinishQuest() {
        DialogueManager.dialogueManager.StartDialogue(DialogueName.FIELD_START_FINISH);
        DialogueManager.dialogueManager.onDialogueFinish.AddListener(TransitionScene);
    }

    private void TransitionScene() {
        LevelManager.levelManager.LoadLevel(LevelName.CUTSCENE_TOWN_START_2, 0);
    }
}
