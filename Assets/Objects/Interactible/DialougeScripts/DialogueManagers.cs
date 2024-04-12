using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
public class DialogueManagers : MonoBehaviour
{
 

    public TMP_Text NameText;
    public TMP_Text DialogueText;
    public GameObject DialogueBox;

    public Queue<string> sentences;

    public bool InDialogue = false;


    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartedDialogue(Dialogue dialogue)
    {
        DialogueBox.SetActive(true);

        //Debug.Log("Starting Conversation W/" + dialogue.name);

        NameText.text = dialogue.name;
         
        sentences.Clear();
        foreach( string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        InDialogue = true;
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        Debug.Log("Tryingto Display Sentence");
        if(sentences.Count == 0)
        {
            StartCoroutine(EndDialogue());
            return;
        }
        Debug.Log("PastIfDisplay");

        string sentenceNew = sentences.Dequeue();
        DialogueText.text = sentenceNew;
    }

    //This is for detection of pressing E the interact key
    public void InputE(InputAction.CallbackContext context)
    {
        if(context.started && InDialogue)
        {
            Debug.Log("INPUTE");
            DisplayNextSentence();
        }
        Debug.Log("IFnotInpt");
    }

    IEnumerator EndDialogue()
    {
        yield return null;//This waits for a frame before entering
        InDialogue=false;
        DialogueBox.SetActive(false);
        Debug.Log("Ending Convo");
    }
}
