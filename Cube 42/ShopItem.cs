using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public bool isEnough;
    public bool isNotEnough;
    public bool isBought;
    public bool isNotBought;

    public GameObject boughtUI;
    public GameObject equippedUI;
    public GameObject unboughtUI;

    public int price;
    public int money;

    public int thisIsEquipped;
    public int thisIsBought;

    public GameObject thisItem;
    public int thisIndex;

    public Text coinsUI;

    public GameObject coins;

    public AudioSource audioSource;

    void Start()
    {
        audioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        money = PlayerPrefs.GetInt("CoinTotal");

        if (price < 10)
        {
            coinsUI.text = "00000" + price.ToString();
        }

        if (price < 100 && price > 10)
        {
            coinsUI.text = "0000" + price.ToString();
        }

        if (price < 1000 && price > 100)
        {
            coinsUI.text = "000" + price.ToString();
        }

        if (price < 10000 && price > 1000)
        {
            coinsUI.text = "00" + price.ToString();
        }

        if (price < 100000 && price > 10000)
        {
            coinsUI.text = "0" + price.ToString();
        }
    }

    void Update()
    {
        if (PlayerPrefs.GetInt(thisIndex.ToString() + "IsBought") == 1)
        {
            this.isBought = true;
            this.isNotBought = false;
        }

        if (this.isNotBought)
        {
            equippedUI.SetActive(false);
            boughtUI.SetActive(false);
        }

        if (this.isNotBought) return;

        thisIsEquipped = PlayerPrefs.GetInt(thisIndex.ToString() + "IsEquipped");
        thisIsBought = PlayerPrefs.GetInt(thisIndex.ToString() + "IsBought");

        if (thisIsEquipped == 1 && PlayerPrefs.GetInt("MeshNumber") == thisIndex)
        {
            equippedUI.SetActive(true);
            boughtUI.SetActive(false);
        }

        if(thisIsBought == 1 && PlayerPrefs.GetInt("MeshNumber") != thisIndex)
        {
            equippedUI.SetActive(false);
            boughtUI.SetActive(true);
        }

        if (thisIsEquipped == 0 && thisIsBought == 0)
        {
            equippedUI.SetActive(false);
            boughtUI.SetActive(false);
        }

        Debug.Log(PlayerPrefs.GetInt(thisIndex.ToString() + "IsEquipped"));
        coins.SetActive(thisIsBought == 0);
    }

    public void SetMesh(int value)
    {
        if (value == 1)
        {
            PlayerPrefs.SetInt("MeshNumber", thisIndex);
        }


        if (value == 2)
        {
            PlayerPrefs.SetInt("MeshNumber", 0);
        }
    }

    public void Buy()
    {
        money = PlayerPrefs.GetInt("CoinTotal");

        if (money < price) return;

        audioSource.Play();

        if (money >= price)
        {
            this.isBought = true;
            this.isNotBought = false;

            int newMoney = PlayerPrefs.GetInt("CoinTotal") - price;
            PlayerPrefs.SetInt("CoinTotal", newMoney);

            PlayerPrefs.SetInt(thisIndex.ToString() + "IsBought", 1);
            PlayerPrefs.SetInt(thisIndex.ToString() + "IsEquipped", 1);
        }

        else
        {
            this.isBought = false;
            this.isNotBought = true;
        }
    }

    public void Equip()
    {
        audioSource.Play();

        PlayerPrefs.SetInt(thisIndex.ToString() + "IsEquipped", 1);
        SetMesh(1);
    }

    public void Unequip()
    {
        audioSource.Play();

        PlayerPrefs.SetInt(thisIndex.ToString() + "IsEquipped", 0);
        SetMesh(2);

        PlayerPrefs.SetInt("MeshNumber", 0);
        PlayerPrefs.SetInt("0IsEquipped", 1);
    }
}
