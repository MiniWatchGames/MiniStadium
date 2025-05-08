using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchingScenePanelController : PanelController
{
    // 현재 보여지고 있는 패널
    public GameObject currentPanel;
    // 전환 패널
    public GameObject nextPanel;
    
    // 패널 전환 대기 시간 (초)
    public float panelDelayTime = 5f;
    // 장면 전환 대기 시간 (초)
    public float sceneDelayTime = 5f;

    // 매칭 중 표시할 문구 텍스트
    public TextMeshProUGUI matchMakingText;
    // 매칭 중 아이콘 (회전 애니메이션)
    public Image progressIcon;

    // Versus 화면 좌/우 플레이어 배경
    public RectTransform playerBackGround;
    public RectTransform enemyBackGround;
    
    // 이동 목표 위치
    public Vector2 playerTargetPos;
    public Vector2 enemyTargetPos;
    
    // ProgressIcon 회전 속도
    public float rotationSpeed = 100f;
    // Versus 배경 이동 속도
    public float moveSpeed = 500f;
    
    // 현재 ProgressIcon이 회전 중인지 회전 여부 확인
    private bool isSpinning = true;
    // Versus 배경이 이동 중인지 이동 여부 확인
    private bool isMovingVersus = false;
    
    // 랜덤으로 보여줄 문구 리스트
    public List<string> tipMessages = new List<string>()
    {
        "확인용 팁 메시지 1",
        "확인용 팁 메시지 2",
        "확인용 팁 메시지 3",
        "확인용 팁 메시지 4",
        "확인용 팁 메시지 5"
    };

    private void Start()
    {
        SetRandomTipMessages(); // 매칭 문구 설정
        StartCoroutine(SwitchPanelAfterDelay()); // 패널 전환 코루틴
    }

    private void Update()
    {
        // ProgressIcon 회전
        if (isSpinning && progressIcon != null)
        {
            progressIcon.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }
        
        /*
        // Versus 패널 이동 애니메이션
        if (isMovingVersus)
        {
            playerBackGround.anchoredPosition = Vector2.MoveTowards(
                playerBackGround.anchoredPosition,
                playerTargetPos,
                moveSpeed * Time.deltaTime);

            enemyBackGround.anchoredPosition = Vector2.MoveTowards(
                enemyBackGround.anchoredPosition,
                enemyTargetPos,
                moveSpeed * Time.deltaTime);

            if (Vector2.Distance(playerBackGround.anchoredPosition, playerTargetPos) < 0.1f
                && Vector2.Distance(enemyBackGround.anchoredPosition, enemyTargetPos) < 0.1f)
            {
                isMovingVersus = false; // 이동 종료
                StartCoroutine(LoadInGameScene()); // 장면 전환 코루틴
            }
        }
        */
    }

    /// <summary>
    /// 랜덤 문구 화면에 설정하는 함수
    /// </summary>
    private void SetRandomTipMessages()
    {
        if (matchMakingText != null && tipMessages.Count > 0)
        {
            int randomIndex = Random.Range(0, tipMessages.Count);
            matchMakingText.text = tipMessages[randomIndex];
        }
    }

    private IEnumerator SwitchPanelAfterDelay()
    {
        yield return new WaitForSeconds(panelDelayTime);
        currentPanel.SetActive(false);
        nextPanel.SetActive(true);
        
        // 회전 멈추기
        StopSpinner();
        
        StartCoroutine(LoadInGameScene());
    }

    /// <summary>
    /// ProgressIcon 회전 중지 함수
    /// </summary>
    private void StopSpinner()
    {
        isSpinning = false;
    }

    private IEnumerator LoadInGameScene()
    {
        yield return new WaitForSeconds(sceneDelayTime);
        SceneManager.LoadScene("IngameScene");
    }
}
