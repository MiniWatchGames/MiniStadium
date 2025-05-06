using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PurchaseManager 
{
    //구매 타이머가 끝났을 때 값을 넣어주어야 함
    private PlayerItems _purchasedPlayerItems;
    public PlayerItems PurchasedPlayerItems
    {
        get => _purchasedPlayerItems;
        set => _purchasedPlayerItems = value;
    }

    public void ResetPurchasedPlayerItems()
    {
        _purchasedPlayerItems = null;
    }

}
