using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PurchaseManager 
{
    private static PlayerItems _purchasedPlayerItems;
    public static PlayerItems PurchasedPlayerItems
    {
        get => _purchasedPlayerItems;
        set => _purchasedPlayerItems = value;
    }

    public static void ResetPurchasedPlayerItems()
    {
        _purchasedPlayerItems = null;
    }

}
