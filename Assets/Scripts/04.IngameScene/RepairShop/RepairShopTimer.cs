using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
public class RepairShopTimer : MonoBehaviour
{
    public Image timerImage;
    public TMP_Text timerText;
    private float timeLimit;
    private float time;
    public float SetTime
    {
        set
        {
            timeLimit = value;
            time = value;
        }
    }

    private Coroutine coTimer;

 
    private void OnEnable() {
        
            coTimer = StartCoroutine(timer());
        
    }

    private void OnDisable()
    {
        if (coTimer != null)
        {
            StopCoroutine(coTimer);
            coTimer = null;
        }
    }

    IEnumerator timer() {
        while (true) {
            timerImage.fillAmount = time / timeLimit;
            timerText.text = $"다음 라운드 시작까지 {time:F1}초...";
            time -= Time.deltaTime;
            yield return null;
        }
    }
}
