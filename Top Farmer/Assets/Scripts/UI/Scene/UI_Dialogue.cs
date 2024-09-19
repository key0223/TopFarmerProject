using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dialogue : MonoBehaviour
{
    [SerializeField] GameObject _dialogueGO;
    [SerializeField] Text _dialogueText;
    [SerializeField] Text _npcNameText;
    public string NpcText { get { return _npcNameText.text; } set { _npcNameText.text = value; } }
    Coroutine CoTyping;

    private void Awake()
    {
        Managers.Dialogue.SetDialogueUI(this);
        _dialogueGO.SetActive(false);
        
    }
    private void Update()
    {
         if(!Managers.Dialogue._dialougeQueue.HasNextDialogue() && (Input.GetKeyDown(KeyCode.Escape)||Input.GetMouseButtonDown(0)))
        {
            Managers.Dialogue.DisableDialogueUI();
        }
    }
    public IEnumerator TypeText(string text, float typeingSpeed)
    {
        _dialogueText.text = "";

        foreach(char letter in text)
        {
            _dialogueText.text += letter;
            yield return new WaitForSeconds(typeingSpeed);
        }
    }

    public void DisplayFullText(string text)
    {
        if (CoTyping != null)
        {
            StopCoroutine(CoTyping);  
        }
        _dialogueText.text = text; 
    }
    public void StartTyping(string text, float typingSpeed)
    {
        if (CoTyping != null)
        {
            StopCoroutine(CoTyping);  // 기존 코루틴 중지
        }
        CoTyping = StartCoroutine(TypeText(text, typingSpeed));  // 새 코루틴 시작
    }
}
