using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignScenePanelController : PanelController
{
    public Button loginButton;
    public Button signupButton;
    public Button signinButton; // 회원가입 패널의 '회원가입' 버튼 (로그인 화면으로 돌아가기)
    public Button backButton;

    private void Start()
    {
        // 버튼에 이벤트 연결
        loginButton.onClick.AddListener(OnLoginClicked);
        signupButton.onClick.AddListener(OnSignupClicked);
        signinButton.onClick.AddListener(OnSigninClicked);
        backButton.onClick.AddListener(OnBackClicked);
        
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
}
