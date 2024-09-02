using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_MailBox : MonoBehaviour
{
    [SerializeField] Text _contentText;
    [SerializeField] MailBox _mailBox;
    [SerializeField] Button _closeButton;
    IMailItem _currentMail;

    private void Start()
    {
        _closeButton.onClick.AddListener(() => OnCloseButtonClicked());
    }
    private void OnEnable()
    {
        _currentMail = _mailBox.GetMail();
        _contentText.text = _currentMail.Content;
    }

    void OnCloseButtonClicked()
    {
        switch(_currentMail.MailType)
        {
            case MailType.Quest:

                break;
            case MailType.Reward:
                break;
        }

        gameObject.SetActive(false); 
    }
}
