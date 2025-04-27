using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignScenePanelController : PanelController
{
    [Header("Buttons")]
    // 로그인 버튼
    public Button loginButton;
    // 회원가입 버튼
    public Button signupButton;
    // 회원가입 버튼 (로그인 화면으로 돌아가기)
    public Button signinButton; 
    // 뒤로가기 버튼
    public Button backButton;
    // 게임종료 버튼
    public Button exitButton;

    private void Start()
    {
        // 버튼에 이벤트 연결
        loginButton.onClick.AddListener(OnLoginClicked);
        signupButton.onClick.AddListener(OnSignupClicked);
        signinButton.onClick.AddListener(OnSigninClicked);
        backButton.onClick.AddListener(OnBackClicked);
        exitButton.onClick.AddListener(OnExitClicked);
        
        // 로그인 패널만
        OpenPanel("[PopupPanel] Login");
    }

    private void OnLoginClicked()
    {
        // 버튼 클릭 시 MainmenuScene으로 전환
        SceneManager.LoadScene("MainmenuScene");
    }

    private void OnSignupClicked()
    {
        // 회원가입 패널 열기
        OpenPanel("[PopupPanel] Signup");
    }

    private void OnSigninClicked()
    {
        // 로그인 패널로 돌아가기
        OpenPanel("[PopupPanel] Login");
    }

    private void OnBackClicked()
    {
        // 로그인 패널로 돌아가기
        OpenPanel("[PopupPanel] Login");
    }
    
    /// <summary>
    /// 게임 종료 버튼 클릭 시 게임 종료 시키는 함수
    /// </summary>
    private void OnExitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
