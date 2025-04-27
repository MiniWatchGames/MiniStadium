using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image image;

    private void Awake()
    {
        health.hp.OnChange += UpdateText;
    }

    private void Reset()
    {
        health = GetComponent<Health>();
    }

    private void UpdateText(float prev, float next, bool asserver)
    {
        image.fillAmount = next / 100f;
    }
}
