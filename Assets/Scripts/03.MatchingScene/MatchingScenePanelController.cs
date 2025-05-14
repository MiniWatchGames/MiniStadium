using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchingScenePanelController : PanelController
{
    [Header("[PopupPanel] MatchMaking")]
    [SerializeField] private TextMeshProUGUI matchingText; // "매칭이진행중입니다..." 문구 텍스트
    [SerializeField] private TextMeshProUGUI gameTipMessage; // 게임팁 문구 텍스트
    [SerializeField] private Image progressBar; // Fill 타입 Image로 설정된 진행바

    [Header("[PopupPanel] MatchFinish")]
    [SerializeField] private GameObject popupMatchFinish; // [PopupPanel] MatchFinish 

    public float fillSpeed = 0.3f; // 진행바 진행 속도
    public float targetProgress = 1f;
    
    private float currentProgress = 0f;
    private bool isFinished = false;
    
    // 랜덤으로 보여줄 게임TIP 문구 리스트
    private List<string> tipMessages = new List<string>()
    {
        "확인용 팁 메시지 1",
        "확인용 팁 메시지 2",
        "확인용 팁 메시지 3",
        "확인용 팁 메시지 4",
        "확인용 팁 메시지 5"
    };

    private void Start()
    {
        // "매칭진행중입니다..." 텍스트 점 개수 반복
        StartCoroutine(AnimationDots()); 
        // 매칭 문구 설정
        SetRandomTipMessages();
    }

    private void Update()
    {
        if (isFinished) return;
        
        if (currentProgress < targetProgress)
        {
            currentProgress += fillSpeed * Time.deltaTime;
            progressBar.fillAmount = currentProgress;

            if (currentProgress >= targetProgress)
            {
                OnMatchComplete();
            }
        }
    }
    
    private void SetRandomTipMessages()
    {
        if (gameTipMessage != null && tipMessages.Count > 0)
        {
            int randomIndex = Random.Range(0, tipMessages.Count);
            gameTipMessage.text = tipMessages[randomIndex];
        }
    }

    private void OnMatchComplete()
    {
        isFinished = true;
        popupMatchFinish.SetActive(true); // [PopupPanel] MatchFinish 패널 띄우기
        StartCoroutine(WaitAndLoadInGame());
    }

    #region 코루틴
    
    IEnumerator AnimationDots()
    {
        string baseText = "매칭이진행중입니다";
        int dotCount = 0;

        while (!isFinished)
        {
            dotCount = (dotCount % 3) + 1;
            matchingText.text = baseText + new string('.', dotCount);

            yield return new WaitForSeconds(0.5f); // 점 변경 시간 간격
        }
    }

    IEnumerator WaitAndLoadInGame()
    {
        yield return new WaitForSeconds(5f); // 5초 대기
        SceneManager.LoadScene("InGameScene"); // InGameScene으로 장면 전환
    }

    #endregion
}
