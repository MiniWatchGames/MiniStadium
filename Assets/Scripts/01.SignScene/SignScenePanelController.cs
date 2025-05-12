using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignScenePanelController : PanelController
{
    [Header("Buttons")]
    [SerializeField] private Button loginButton; // 로그인 버튼
    [SerializeField] private Button signupButton; // 회원가입 화면으로 가는 버튼
    [SerializeField] private Button signinButton; // 회원가입 버튼
    [SerializeField] private Button backLoginButton; // 로그인 화면으로 가는 버튼
    [SerializeField] private Button exitButton; // 게임종료 버튼

    [Header("Signup Input Fields")]
    [SerializeField] private TMP_InputField signupIdInput; // 아이디 입력 필드
    [SerializeField] private TMP_InputField signupNickNameInput; // 닉네임 입력 필드
    [SerializeField] private TMP_InputField signupPasswordInput; // 비밀번호 입력 필드
    [SerializeField] private TMP_InputField signupCheckPasswordInput; // 비밀번호 확인용 입력 필드

    [Header("Signup Success Popup")]
    // 회원가입 성공 팝업 패널
    public GameObject successPopupPanel;
    // 회원가입 성공 팝업 패널 메시지
    public TMP_Text successMessageText;
    // 회원가입 성공 후 로그인 패널로 돌아가는 버튼
    public Button goLoginButton;

    [Header("Signup Fail Popup")]
    // 회원가입 실패 팝업 패널
    public GameObject failPopupPanel;
    // 회원가입 실패 팝업 패널 메시지
    public TMP_Text failMessageText;
    // 회원가입 실패 후 팝업 패널 닫는 버튼
    public Button closeFailPopupButton;
    
    // 테스트용 (이미 존재하는 아이디 목록) -> DB로 대체
    private List<string> testUserIds = new List<string> { "admin1", "admin2", "admin3" };
    
    private void Start()
    {
        // 버튼 클릭 이벤트 연결
        loginButton.onClick.AddListener(OnLoginClicked);
        signupButton.onClick.AddListener(OnSignupClicked);
        signinButton.onClick.AddListener(OnSigninClicked);
        backLoginButton.onClick.AddListener(OnBackClicked);
        exitButton.onClick.AddListener(OnExitClicked);
        
        // 성공 팝업 패널 내 로그인 버튼 클릭 이벤트 연결
        if (goLoginButton != null)
            goLoginButton.onClick.AddListener(PopupLoginClicked);
        
        // 실패 팝업 패널 내 닫기 버튼 클릭 이벤트 연결
        if (closeFailPopupButton != null)
            closeFailPopupButton.onClick.AddListener(CloseFailPopup);
        
        // 시작 화면 : 로그인 패널
        OpenPanel("[PopupPanel] Login");
        
        // 성공/실패 팝업 패널 비활성화
        if (successPopupPanel != null) successPopupPanel.SetActive(false);
        if (failPopupPanel != null) failPopupPanel.SetActive(false);
    }
    
    #region 장면 전환 + 패널 이동 함수
    
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
    
    #endregion

    #region 회원가입 에러 메시지

    private bool TrySignUp(string userId, string userNickname, string password, string checkPassword, out string errorMessage)
    {
        errorMessage = "";
        return true;
    }

    #endregion

    #region 성공/실패 팝업 패널 함수

    private void ShowSuccessPopup(string message)
    {
        if (successPopupPanel != null && successMessageText != null)
        {
            successMessageText.text = message;
            successPopupPanel.SetActive(true);
        }
    }

    private void ShowFailPopup(string message)
    {
        if (failPopupPanel != null && failMessageText != null)
        {
            failMessageText.text = message;
            failPopupPanel.SetActive(true);
        }
    }

    private void CloseFailPopup()
    {
        if (failPopupPanel != null)
            failPopupPanel.SetActive(false);
    }

    private void PopupLoginClicked()
    {
        if (successPopupPanel != null)
            successPopupPanel.SetActive(false);
        OpenPanel("[PopupPanel] Login");
    }

    #endregion

    #region 게임 종료 함수

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

    #endregion
}
