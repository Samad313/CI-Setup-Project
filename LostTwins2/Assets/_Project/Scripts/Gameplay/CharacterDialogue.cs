using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDialogue : MonoBehaviour
{
    [SerializeField]
    private DialogueType myDialogueType = default;

    public DialogueType MyDialogueType { get { return myDialogueType; } }
}
