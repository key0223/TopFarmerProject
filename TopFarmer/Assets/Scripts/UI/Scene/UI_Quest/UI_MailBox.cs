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
        if (_mailBox == null)
            return;

        string mailId = Managers.Mail.DequeueMail();
        if(mailId == null)
        {
            _contentText.text = "받은 편지가 없습니다.";
        }
        else
        {
            ShowMail(mailId);
        }
    }
    void ShowMail(string mailId)
    {
        string mail = Managers.Data.StringDict[mailId].ko;
        int index = mail.IndexOf("[#]");
        if (index != -1)
        {
            mail = mail.Substring(0, index);
        }
        mail = mail.Replace("@", PlayerController.Instance.FarmerName);


        if(mail.Contains("%item"))
        {
            string oldValue = mail.Substring(mail.IndexOf("%item"), mail.IndexOf("%%") + 2 - mail.IndexOf("%item"));
            string[] strArray1 = oldValue.Split(' ');
            mail = mail.Replace(oldValue, "");
            mail = mail.Replace("^", "\n");
            _contentText.text = mail;

            if (strArray1[1].Equals("object"))
            {
                int maxValue = strArray1.Length - 1;
                int num =Random.Range(3, maxValue);
                int itemId = num - num % 2;

                // TODO : Add to inventory
            }
        }

    }
    void OnCloseButtonClicked()
    {
        gameObject.SetActive(false); 
    }
}
