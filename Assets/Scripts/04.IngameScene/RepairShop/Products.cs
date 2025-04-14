using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Products
{
    public string name;
    public GameObject priceText;
    public int price;
    
    public Products(string name, int price, GameObject priceText)
    {
        this.name = name;
        this.price = price;
        this.priceText = priceText;
    }
}


