using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, Interactable
{
    public Dialogue dialogue;
    Animator anim;

    /*public void Interact(Entity source)
    {
        //throw new System.NotImplementedException();
    }*/

    public void Interact(Entity source)
    {
        // Call the TriggerDialogue method to start the dialogue when interacted with
        if(source.IsLocalPlayer)
        {
            TriggerDialogue();
        }
    }



    public void TriggerDialogue()
    {
       FindObjectOfType<DialogueManagers>().StartedDialogue(dialogue);
       
    
    }


}
