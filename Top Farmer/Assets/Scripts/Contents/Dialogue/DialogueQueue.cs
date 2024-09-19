using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueQueue 
{
    Queue<Dialogue> _dialogueQueue = new Queue<Dialogue>();

    public void EnqueueDialogue(Dialogue dialogue)
    {
        _dialogueQueue.Enqueue(dialogue);
    }

    public Dialogue DequeueDialogue()
    {
        if(_dialogueQueue.Count > 0)
            return _dialogueQueue.Dequeue();
        return null;
    }

    public bool HasNextDialogue()
    {
        return _dialogueQueue.Count > 0;
    }
}
