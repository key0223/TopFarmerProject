using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_MailBox : MonoBehaviour
{
    [SerializeField] Text _contentText;
    [SerializeField] Button _closeButton;
    [SerializeField] GameObject _receiveItemGO;
    [SerializeField] Image _receiveItemImage;
    [SerializeField] Text _receiveItemQuantityText;

    int _receiveItemId;
    int _receiveItemQuantity;
    private void Start()
    {
        _closeButton.onClick.AddListener(() => OnCloseButtonClicked());
    }
    private void OnEnable()
    {
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
    private void OnDisable()
    {
        OnUIClose();
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
                int itemIdIndex = num - num % 2;

                _receiveItemId = int.Parse(strArray1[itemIdIndex]);
                _receiveItemQuantity = int.Parse(strArray1[itemIdIndex + 1]);

                ItemData itemData = Managers.Data.GetItemData(_receiveItemId);

                if(itemData != null)
                {
                    _receiveItemImage.sprite = Managers.Data.SpriteDict[itemData.itemSpritePath];
                    _receiveItemQuantityText.text = _receiveItemQuantity.ToString();
                    _receiveItemGO.SetActive(true);
                }
                // TODO : Add to inventory
            }
        }

    }

    void OnUIClose()
    { 
        if(_receiveItemId >0)
        {
            InventoryManager.Instance.AddItem(InventoryType.INVEN_PLAYER, _receiveItemId);
            _receiveItemImage.sprite = null;
            _receiveItemQuantityText.text = "";
            _receiveItemId = -1;
            _receiveItemQuantity = -1;
            _receiveItemGO.SetActive(false);

        }

    }

    void OnCloseButtonClicked()
    {
        gameObject.SetActive(false); 
    }
}
