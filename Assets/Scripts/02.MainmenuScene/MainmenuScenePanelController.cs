using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainmenuScenePanelController : PanelController
{
    public Button leftArrowButton;
    public Button rightArrowButton;
    public Button categoryButton;
    public Button exitButton;

    public TextMeshProUGUI categoryText;

    private string[] categories = { "게임 시작", "랭킹", "설정" };
    private int currentIndex = 0;

    private void Start()
    {
        leftArrowButton.onClick.AddListener(OnLeftArrowClicked);
        rightArrowButton.onClick.AddListener(OnRightArrowClicked);
        categoryButton.onClick.AddListener(OnCategoryClicked);
        exitButton.onClick.AddListener(OnExitClicked);
        
        // 처음 텍스트 설정
        UpdateCategoryText();
    }

    private void OnLeftArrowClicked()
    {
        currentIndex = (currentIndex - 1 + categories.Length) % categories.Length;
        Debug.Log("왼쪽 버튼을 눌렀습니다.");
        Debug.Log("현재 카테고리: " + categories[currentIndex]);
        UpdateCategoryText();
    }

    private void OnRightArrowClicked()
    {
        currentIndex = (currentIndex + 1) % categories.Length;
        Debug.Log("오른쪽 버튼을 눌렀습니다.");
        Debug.Log("현재 카테고리: " + categories[currentIndex]);
        UpdateCategoryText();
    }

    private void OnCategoryClicked()
    {
        Debug.Log("카테고리 버튼 클릭: " + categories[currentIndex]);

        switch (categories[currentIndex])
        {
            case "게임 시작":
                // 버튼 클릭 시 MatchingScene으로 전환
                SceneManager.LoadScene("MatchingScene");
                break;
            case "랭킹":
                OpenPanel("[PopUpPanel] Ranking");
                break;
            case "설정":
                OpenPanel("[PopUpPanel] Settings");
                break;
        }
    }
    
    private void UpdateCategoryText()
    {
        categoryText.text = categories[currentIndex];
    }

    public void CloseRankingPanel()
    {
        ClosePanel("[PopUpPanel] Ranking");
    }

    private void CloseSettingsPanel()
    {
        ClosePanel("[PopUpPanel] Settings");
    }

    private void OnExitClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
