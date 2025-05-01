using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RepairShopTimer : MonoBehaviour
{
    public Image timerImage;
    public TMP_Text timerText;
    public float timeLimit;
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        timeLimit = time;
    }

    // Update is called once per frame
    void Update()
    {
        timerImage.fillAmount = time / timeLimit;
        timerText.text = $"다음 라운드 시작까지 {time:F1}초...";
    }
}
