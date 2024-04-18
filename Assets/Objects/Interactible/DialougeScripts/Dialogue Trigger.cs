using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, Interactable
{
    public Dialogue dialogue;
    public bool disableAfterTrigger = false;
    Animator anim;
    bool activeDialogue = true;

    /*public void Interact(Entity source)
    {
        //throw new System.NotImplementedException();
    }*/

    public void Interact(Entity source)
    {
        // Call the TriggerDialogue method to start the dialogue when interacted with
        if(source.IsLocalPlayer)
        {
            if (activeDialogue)
                TriggerDialogue();
            if (disableAfterTrigger)
                activeDialogue = false;
        }
    }



    public void TriggerDialogue()
    {
       FindObjectOfType<DialogueManagers>().StartedDialogue(dialogue);

       if (TryGetComponent<Animator>(out anim))
        {
            anim.SetTrigger("Converse");
        }
    }


}
