using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public DialogueQueue _dialougeQueue;
    public UI_Dialogue DialogueUI {  get; private set; }

    float _typingSpeed = 0.05f;
   
   
    public void SetDialogueUI(UI_Dialogue dialogueUI)
    {
        DialogueUI = dialogueUI;
    }
    public void SetDialougeQueue(DialogueQueue dialougeQueue)
    {
        _dialougeQueue = dialougeQueue;
        EnableDialogueUI();
        StartDialogue();
    }

    public void MakeDialogueQueue(string stringData)
    {
        DialogueQueue newQueue = new DialogueQueue();

        string[] dialogues = stringData.Split("#b");

        foreach(string dialogue in dialogues)
        {
            Dialogue newDialogue = new Dialogue(dialogue);

            newQueue.EnqueueDialogue(newDialogue);
        }

        SetDialougeQueue(newQueue);

    }
    public void StartDialogue()
    {
        Dialogue currentDialogue = _dialougeQueue.DequeueDialogue();

        if (currentDialogue != null)
        {
            string currentText = currentDialogue.Text;
            //DialogueUI.StartTyping(currentText, _typingSpeed);
            DialogueUI.DisplayFullText(currentText);
        }
    }

    public void EnableDialogueUI()
    {
        DialogueUI.gameObject.SetActive(true);
        PlayerController.Instance.PlayerInputDisabled = true;
        Time.timeScale = 0;
     
    }

    public void DisableDialogueUI()
    {
        DialogueUI.gameObject.SetActive(false);
        PlayerController.Instance.PlayerInputDisabled = false;
        Time.timeScale = 1;
        _dialougeQueue = null;
       
    }
}
