using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour//, Interactable
{
    public Dialogue dialogue;
    Animator anim;

    public void TriggerDialogue()
    {
       FindObjectOfType<DialogueManagers>().StartedDialogue(dialogue);
       
    
    }


}
