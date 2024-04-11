using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogueManagers : MonoBehaviour
{
    public Text NameText;
    public Text DialogueText;
    private Queue<string> sentences;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartedDialogue(Dialogue dialogue)
    {
        //Debug.Log("Starting Conversation W/" + dialogue.name);
    
        NameText.text = dialogue.name;
         
        sentences.Clear();
        foreach( string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentenceNew = sentences.Dequeue();
        DialogueText.text = sentenceNew;
    }

    void EndDialogue()
    {
        Debug.Log("Ending Convo");
    }
}
