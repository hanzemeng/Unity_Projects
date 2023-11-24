using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class DialogueName
{
    public static string CUTSCENE_SWAMP_4 = "Cutscene_Swamp_Floor4";
    public static string CUTSCENE_HELL_TRENCH_8_1 = "Cutscene_HellTrench_Floor8_1";
    public static string CUTSCENE_HELL_TRENCH_8_2 = "Cutscene_HellTrench_Floor8_2";
    public static string CUTSCENE_HELL_1 = "Cutscene_Hell_1";
    public static string CUTSCENE_HELL_2 = "Cutscene_Hell_2";
    public static string CUTSCENE_EPILOGUE = "Cutscene_Epilogue";
    public static string FIELD_START_FINISH = "field_start_3";
    public static string[] DIALOGUE_IMAGES =
                                            {
                                                "IntroDialogueBubble",
                                                "NormalTextBubble",
                                                "NormalTextBubble_NoSpeaker_V2",
                                                "NormalTextBubble_V2",
                                                "ShoutTextBubble",
                                                "ShoutTextBubble_V2",
                                                "ThoughtTextBubble",
                                                "ThoughtTextBubble_V2",

                                                "Empty_Portrait",
                                                "NPC_Portrait",
                                                "Hero_Portrait",
                                                "Axorath_Portrait",
                                                "Elder_Dendrius_Portrait",
                                                "NPC_Female_1_Portrait",
                                                "NPC_Female_2_Portrait",
                                                "NPC_Male_1_Portrait",
                                                "NPC_Male_2_Portrait",
                                                "NPC_Soldier_Portrait"
                                            };
}


/*
    StartDialogue(dialogueName):
        Start a dialogue if and only if no other dialogue is playing.
        The content of the dialogue is defined in the file called dialogueName.
        Please see Resource/Dialogue/Syntax to understand how to create a dialogue file.
    EndDialogue():
        End the current dialogue. This function is automatically called when the current dialogue reaches the end.
*/

class StringValueComparator : IEqualityComparer<string>
{
    public bool Equals(string x, string y)
    {
        if (x.Length != y.Length)
        {
            return false;
        }
        for(int i=0; i<x.Length; i++)
        {
            if(x[i] != y[i])
            {
                return false;
            }
        }
        return true;
    }
    public int GetHashCode(string x)
    {
        return x.GetHashCode();
    }
}

public class DialogueManager : MonoBehaviour
{
    private PlayerInputs playerInputs;

    public static DialogueManager dialogueManager;
    private string DIALOGUE_FOLDER = "Dialogue";
    private string DIALOGUE_IMAGE_FOLDER = "DialogueImage";

    private Dictionary<string, Sprite> dialogueImages;

    [SerializeField] private Image textBoxImage;
    [SerializeField] private Text textBoxText;
    [SerializeField] private Image nextButtonImage;
    [SerializeField] private Image characterLeftImage;
    [SerializeField] private Image characterRightImage;
    [SerializeField] private Canvas canvas; 
    private TweenerColor textBoxImageTweener;
    private TweenerColor nextButtonImageTweener;
    private TweenerColor characterLeftImageTweener;
    private TweenerColor characterRightImageTweener;

    private bool pauseGame;

    private string[] textLines;
    private int textLinesIndex;

    private IEnumerator fillTextBoxCoroutine;
    private bool isFillingTextBox;

    [HideInInspector] public UnityEvent onDialogueFinish = new UnityEvent();

    private void Awake()
    {
        if(null == dialogueManager)
        {
            dialogueManager = this;
        }
        else
        {
            Destroy(gameObject);
        }

        textLinesIndex = -1;

        dialogueImages = new Dictionary<string, Sprite>(new StringValueComparator());
        foreach(string s in DialogueName.DIALOGUE_IMAGES)
        {
            string imageFilePath = Path.Combine(DIALOGUE_IMAGE_FOLDER, s);
            dialogueImages[s] = Resources.Load<Sprite>(imageFilePath);
        }

        textBoxImageTweener = new TweenerColor(this, res=>textBoxImage.color=res);
        nextButtonImageTweener = new TweenerColor(this, res=>nextButtonImage.color=res);
        characterLeftImageTweener = new TweenerColor(this, res=>characterLeftImage.color=res);
        characterRightImageTweener = new TweenerColor(this, res=>characterRightImage.color=res);
        isFillingTextBox = false;
    }

    private void Start() {
        playerInputs = PlayerInputManager.playerInputs;
        playerInputs.DialogueAction.LeftClick.canceled += UpdateTextBox;
        playerInputs.DialogueAction.Disable();
    }

    public void StartDialogue(string dialogueName, bool pauseGame=true)
    {
        if(-1 != textLinesIndex)
        {
            Debug.Log("a dialogue is already in progress");
            return;
        }

        string textFilePath = Path.Combine(DIALOGUE_FOLDER, dialogueName);
        TextAsset currentText = Resources.Load<TextAsset>(textFilePath);
        if(null == currentText)
        {
            Debug.Log("failed to load " + textFilePath);
            return;
        }


        textBoxImageTweener.TweenWithTime(textBoxImage.color, new Color(1f,1f,1f,1f), 0.5f, Tweener.LINEAR);
        characterLeftImageTweener.TweenWithTime(characterLeftImage.color, new Color(1f,1f,1f,1f), 0.5f, Tweener.LINEAR);
        characterRightImageTweener.TweenWithTime(characterRightImage.color, new Color(1f,1f,1f,1f), 0.5f, Tweener.LINEAR);

        textLines = currentText.text.Split(new [] { '\n' });
        textLinesIndex = 0;

        this.pauseGame = pauseGame;
        if(this.pauseGame)
        {
            playerInputs.DialogueAction.Enable();
            playerInputs.MenuAction.Disable();
            PauseHandler.current.Pause(false);
        }
        
        canvas.enabled = true;
        UpdateTextBox(new InputAction.CallbackContext());
    }

    public void EndDialogue()
    {
        textBoxImageTweener.TweenWithTime(textBoxImage.color, new Color(1f,1f,1f,0f), 0.5f, Tweener.LINEAR);
        nextButtonImageTweener.TweenWithTime(nextButtonImage.color, new Color(1f,1f,1f,0f), 0.5f, Tweener.LINEAR);
        characterLeftImageTweener.TweenWithTime(characterLeftImage.color, new Color(1f,1f,1f,0f), 0.5f, Tweener.LINEAR);
        characterRightImageTweener.TweenWithTime(characterRightImage.color, new Color(1f,1f,1f,0f), 0.5f, Tweener.LINEAR);

        if(isFillingTextBox)
        {
            StopCoroutine(fillTextBoxCoroutine);
            isFillingTextBox = false;
        }
        textBoxText.text  = "";
        textLinesIndex = -1;
        AudioManager.instance.PauseBackgroundSFX(AudioManager.SoundResource.TEXT_CRAWLING_SFX);

        if(this.pauseGame)
        {
            playerInputs.DialogueAction.Disable();
            playerInputs.MenuAction.Enable();
            PauseHandler.current.Resume();
        }

        canvas.enabled = false;

        onDialogueFinish.Invoke();
        onDialogueFinish.RemoveAllListeners();
    }

    public void UpdateTextBox(InputAction.CallbackContext callbackContext)
    {
        if(-1 == textLinesIndex)
        {
            return;
        }

        string[] currentLineSetting = textLines[textLinesIndex].Split(new [] { ' ' });
        string currentLine = textLines[textLinesIndex+1];
        if("" == currentLine)
        {
            EndDialogue();
            return;
        }
        currentLine = ReplaceName(currentLine);

        if("Left" == currentLineSetting[0])
        {
            textBoxImage.rectTransform.eulerAngles = new Vector3(0f, 0, 0f);
        }
        else
        {
            textBoxImage.rectTransform.eulerAngles = new Vector3(0f, 180f, 0f);
        }

        textBoxImage.sprite = dialogueImages[currentLineSetting[1].Trim()];
        characterLeftImage.sprite = dialogueImages[currentLineSetting[2].Trim()];
        characterRightImage.sprite = dialogueImages[currentLineSetting[3].Trim()];

        if(isFillingTextBox)
        {
            StopCoroutine(fillTextBoxCoroutine);
            isFillingTextBox = false;
            textBoxText.text = currentLine;
            textLinesIndex += 2;
            nextButtonImageTweener.TweenWithTime(nextButtonImage.color, new Color(1f,1f,1f,1f), 0.25f, Tweener.LINEAR);
            AudioManager.instance.PauseBackgroundSFX(AudioManager.SoundResource.TEXT_CRAWLING_SFX);
        }
        else
        {
            fillTextBoxCoroutine = FillTextBoxCoroutine(currentLine);
            StartCoroutine(fillTextBoxCoroutine);
        }   
    }

    private string ReplaceName(string originalString)
    {
        string res = "";
        bool shouldAppend = true;
        foreach(char c in originalString)
        {
            if('{' == c)
            {
                shouldAppend = false;
                continue;
            }
            else if('}' == c)
            {
                shouldAppend = true;
                res += SaveLoadManager.saveLoadManager.activeSaveSlot.saveName;
                continue;
            }

            if(shouldAppend)
            {
                res += c;
            }

        }
        return res;
    }

    private IEnumerator FillTextBoxCoroutine(string line)
    {
        isFillingTextBox = true;

        AudioManager.instance.PlayBackgroundSFX(AudioManager.SoundResource.TEXT_CRAWLING_SFX);
        nextButtonImageTweener.TweenWithTime(nextButtonImage.color, new Color(1f,1f,1f,0f), 0.1f, Tweener.LINEAR);

        string plainText = "";
        bool isIgnoring = false;
        foreach(char c in line)
        {
            if('<' == c)
            {
                isIgnoring = true;
                continue;
            }
            else if('>' == c)
            {
                isIgnoring = false;
                continue;
            }
            if(isIgnoring)
            {
                continue;
            }
            plainText += c;
        }

        string formattedText = "";
        List<(string, string)> styles = new List<(string, string)>();
        int plainTextIndex = 0;

        for(int i=0; i<line.Length; i++)
        {
            while('<' == line[i])
            {
                string property = "";
                string value = "";
                bool isClosing = false;
                bool hasValue = false;

                i++;
                if('/' == line[i])
                {
                    styles.RemoveAt(styles.Count-1);
                    isClosing = true;
                }
                while('>' != line[i])
                {
                    if('=' == line[i])
                    {
                        hasValue = true;
                        i++;
                        continue;
                    }
                    if(!isClosing)
                    {
                        if(hasValue)
                        {
                            value += line[i];
                        }
                        else
                        {
                            property += line[i];
                        }
                    }
                    i++;
                }
                if(!isClosing)
                {
                    styles.Add((property, value));
                }
                i++;
                if(i >= line.Length)
                {
                    goto RETURN;
                }
            }

            foreach((string, string) style in styles)
            {
                formattedText += "<";
                formattedText += style.Item1;
                if("" != style.Item2)
                {
                    formattedText += "=";
                    formattedText += style.Item2;
                }
                formattedText += ">";
            }
            formattedText += plainText[plainTextIndex];
            plainTextIndex++;
            for(int j=styles.Count-1; j>=0; j--)
            {
                formattedText += "</";
                formattedText += styles[j].Item1;
                formattedText += ">";
            }

            textBoxText.text = formattedText + "<color=#FFFFFF00>" + plainText.Substring(plainTextIndex) + "</color>";
            yield return new WaitForSecondsRealtime(0.025f);
        }

        RETURN:
        textLinesIndex += 2;
        nextButtonImageTweener.TweenWithTime(nextButtonImage.color, new Color(1f,1f,1f,1f), 0.25f, Tweener.LINEAR);
        AudioManager.instance.PauseBackgroundSFX(AudioManager.SoundResource.TEXT_CRAWLING_SFX);

        isFillingTextBox = false;
    }

    public void UpdateTextBox()
    {
        if(-1 == textLinesIndex)
        {
            return;
        }

        string[] currentLineSetting = textLines[textLinesIndex].Split(new [] { ' ' });
        string currentLine = textLines[textLinesIndex+1];
        if("" == currentLine)
        {
            EndDialogue();
            return;
        }
        currentLine = ReplaceName(currentLine);
        if("Left" == currentLineSetting[0])
        {
            textBoxImage.rectTransform.eulerAngles = new Vector3(0f, 0, 0f);
        }
        else
        {
            textBoxImage.rectTransform.eulerAngles = new Vector3(0f, 180f, 0f);
        }

        textBoxImage.sprite = dialogueImages[currentLineSetting[1].Trim()];
        characterLeftImage.sprite = dialogueImages[currentLineSetting[2].Trim()];
        characterRightImage.sprite = dialogueImages[currentLineSetting[3].Trim()];

        if(isFillingTextBox)
        {
            StopCoroutine(fillTextBoxCoroutine);
            isFillingTextBox = false;
            textBoxText.text = currentLine;
            textLinesIndex += 2;
            if(nextButtonImage.IsActive())
            {
                nextButtonImageTweener.TweenWithTime(nextButtonImage.color, new Color(1f,1f,1f,1f), 0.25f, Tweener.LINEAR);
            }
            AudioManager.instance.PauseBackgroundSFX(AudioManager.SoundResource.TEXT_CRAWLING_SFX);
        }
        else
        {
            fillTextBoxCoroutine = FillTextBoxCoroutine(currentLine);
            StartCoroutine(fillTextBoxCoroutine);
        }   
    }


}
