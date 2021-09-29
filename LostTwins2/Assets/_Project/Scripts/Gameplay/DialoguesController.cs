using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogueType {Bulb, Exclamation, Question }

public class DialoguesController : MonoBehaviour
{
    public static DialoguesController instance;

    [SerializeField]
    private Transform dialogue = default;

    [SerializeField]
    private Vector3 offset = default;

    private Transform[] dialogues;

    [SerializeField]
    private float showDialogueTime = default;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        dialogues = new Transform[2];

        for (int i = 0; i < 2; i++)
        {
            dialogues[i] = Instantiate(dialogue);
            dialogues[i].parent = GameplayManager.instance.GetAllPlayers()[i].GetMyVisual().transform;
            dialogues[i].localPosition = offset;
            dialogues[i].Find("BG").gameObject.SetActive(false);
            foreach (Transform item in dialogues[i].Find("Reactions"))
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    public void ShowDialogue(DialogueType dialogueType, CharName charName, float showTime, Vector3 positionOffset)
    {
        ShowDialogueFunctionality(dialogueType, charName, showTime, positionOffset);
    }

    public void ShowDialogue(DialogueType dialogueType, CharName charName, float showTime)
    {
        ShowDialogueFunctionality(dialogueType, charName, showTime, offset);
    }

    public void ShowDialogue(DialogueType dialogueType, CharName charName)
    {
        ShowDialogueFunctionality(dialogueType, charName, showDialogueTime, offset);
    }

    private void ShowDialogueFunctionality(DialogueType dialogueType, CharName charName, float showTime, Vector3 positionOffset)
    {
        GameObject currentDialogue = null;

        dialogues[(int)charName].Find("BG").gameObject.SetActive(true);
        dialogues[(int)charName].localPosition = positionOffset;
        foreach (Transform item in dialogues[(int)charName].Find("Reactions"))
        {
            if(item.GetComponent<CharacterDialogue>().MyDialogueType== dialogueType)
            {
                currentDialogue = item.gameObject;
                currentDialogue.SetActive(true);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }

        StartCoroutine(DisableDialogueAfterSomeTime(dialogues[(int)charName].gameObject, showTime));
    }

    private IEnumerator DisableDialogueAfterSomeTime(GameObject currentDialogueGroup, float showTime)
    {
        float t = 0.0f;
        float actualT = 0.0f;

        while(t<1.0f)
        {
            actualT = Easing.Ease(Equation.ElasticEaseOut, t, 0, 1, 1);
            currentDialogueGroup.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, actualT);
            t += Time.deltaTime * 1.5f;
            yield return new WaitForEndOfFrame();
        }

        currentDialogueGroup.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(showTime);

        t = 1.0f;

        while (t > 0.0f)
        {
            actualT = Easing.Ease(Equation.SineEaseOut, t, 0, 1, 1);
            currentDialogueGroup.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, actualT);
            t -= Time.deltaTime * 6.5f;
            yield return new WaitForEndOfFrame();
        }

        foreach (Transform item in currentDialogueGroup.transform.Find("Reactions"))
        {
            item.gameObject.SetActive(false);
        }
        currentDialogueGroup.transform.Find("BG").gameObject.SetActive(false);
    }
}
