using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Gold : MonoBehaviour
{

    [SerializeField] Sprite[] _numberSprites;
    [SerializeField] Image[] _numberImages;

    private void Awake()
    {
        Managers.Event.UpdatePlayerCoinEvent -= UpdateCoinImages;
        Managers.Event.UpdatePlayerCoinEvent += UpdateCoinImages;
    }

    private void Start()
    {
        UpdateCoinImages();
    }
    public void UpdateCoinImages()
    {
        string coinString = PlayerController.Instance.PlayerCoin.ToString();

        for (int i = 0; i < _numberImages.Length; i++)
        {
            if(i<coinString.Length)
            {
                int number = int.Parse(coinString[coinString.Length - i - 1].ToString());
                _numberImages[i].sprite = _numberSprites[number];
                _numberImages[i].enabled = true;   
            }
            else
            {
                _numberImages[i].enabled = false;
            }
        }
    }
}
