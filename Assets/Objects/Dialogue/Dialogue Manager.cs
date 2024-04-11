using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    string SpeakerName;
    Queue<string> sentences;
    public bool InDialogue;
    bool displayingSentence;

    void StartDialogue()
    {

    }

    void ContinueDialogue() { 
    
    }

    IEnumerator DisplaySentence(string message) {
        yield return null;
    }

    IEnumerator EndDialogue() {
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void StartDialogue(Dialogue dialogue)
    {
        throw new NotImplementedException();
    }
}
