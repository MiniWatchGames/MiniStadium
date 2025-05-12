using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignScenePanelController : PanelController
{
    private class UserInfo
    {
        public string Id;
        public string Password;
        public string Nickname;

        public UserInfo(string id, string password, string nickname)
        {
            Id = id;
            Password = password;
            Nickname = nickname;
        }
    }
    
    [Header("System Buttons")] 
    [SerializeField] private Button optionButton; // 게임 옵션 버튼
    [SerializeField] private Button exitButton; // 게임 종료 버튼

    [Header("LoginForm Elements")] 
    [SerializeField] private TMP_InputField loginIdField; // 로그인 패널 아이디 입력 필드
    [SerializeField] private TMP_InputField loginPasswordField; // 로그인 패널 비밀번호 입력 필드
    [SerializeField] private TMP_Text loginFailText; // 로그인 실패 문구
    [SerializeField] private Button loginButton; // 로그인 버튼
    [SerializeField] private Button signupButton; // 회원가입 화면으로 가는 버튼
    [SerializeField] private Button checkBoxButton; // 로그인 유지 체크박스 버튼
    [SerializeField] private GameObject checkBoxMark; // 체크박스 표시 오브젝트

    [Header("SignUpForm Elements")] 
    [SerializeField] private TMP_InputField signupIdField; // 회원가입 패널 아이디 입력 필드
    [SerializeField] private TMP_InputField signupNickNameInputField; // 회원가입 패널 닉네임 입력 필드
    [SerializeField] private TMP_InputField signupPasswordField; // 회원가입 패널 비밀번호 입력 필드
    [SerializeField] private TMP_InputField signupCheckPasswordInputField; // 회원가입 패널 비밀번호 확인용 입력 필드
    [SerializeField] private TMP_Text signupFailText; // 회원가입 실패 문구
    [SerializeField] private Button signinButton; // 회원가입 완료 버튼
    [SerializeField] private Button goLoginPanelButton1; // 회원가입 패널에서 로그인 패널로 돌아가는 버튼

    [Header("Signup Success Element")] 
    [SerializeField] private Button goLoginPanelButton2; // 회원가입 성공 패널에서 로그인 패널로 돌아가는 버튼

    [Header("OptionSetting Element")] 
    [SerializeField] private Button optionCloseButton; // 옵션 패널 닫는 버튼 (옵션 패널 -> 로그인 화면)
    
    // 로그인 유지 여부 저장 변수
    private bool isChecked = false; 
    
    // 회원가입한 유저 정보를 저장하는 리스트(임시)
    private List<UserInfo> signupUserList = new List<UserInfo>();
    
    // 임시 테스트용 유저 목록 (실제로는 DB 사용 예정)
    private List<UserInfo> testUserList = new List<UserInfo>
    {
        new UserInfo("admin1", "1234", "관리자1"),
        new UserInfo("admin2", "1234", "관리자2"),
        new UserInfo("admin3", "1234", "관리자3")
    };

    private const string StayLoggedInKey = "StayLoggedIn";
    private const string SavedLoginIdKey = "SavedLoginId";
    private const string SavedLoginPwKey = "SavedLoginPw";
    
    private void Start()
    {
        // 시작 화면 : 로그인 패널
        OpenPanel("[PopupPanel] Login");
        // SignScene에 있는 버튼 클릭 이벤트 연결 함수
        OnClickButtons();
        
        // 체크박스 저장된 상태 불러오기
        isChecked = PlayerPrefs.GetInt("StayLoggedIn", 0) == 1;
        UpdateVisual();
        
        // 저장된 로그인 정보 불러오기
        if (isChecked)
        {
            loginIdField.text = PlayerPrefs.GetString(SavedLoginIdKey, "");
            loginPasswordField.text = PlayerPrefs.GetString(SavedLoginPwKey, "");
        }
        else
        {
            loginIdField.text = "";
            loginPasswordField.text = "";
        }
        
        // 로그인 실패 문구 숨기기
        if (loginFailText != null) loginFailText.gameObject.SetActive(false);
        // 회원가입 실패 문구 숨기기
        if (signupFailText != null) signupFailText.gameObject.SetActive(false);
    }
    
    #region 버튼 클릭 이벤트 연결 함수

    private void OnClickButtons()
    {
        // 로그인 유지 체크박스 버튼 이벤트 연결 (로그인 패널)
        checkBoxButton.onClick.AddListener(ToggleCheckBox);
        // 로그인 버튼 이벤트 연결 (로그인 패널 -> 장면전환)
        loginButton.onClick.AddListener(OnLoginClicked);
        // 회원가입 화면으로 가는 버튼 이벤트 연결 (로그인 패널 -> 회원가입 패널)
        signupButton.onClick.AddListener(OnSignupClicked);
        // 회원가입 완료 버튼 이벤트 연결 (회원가입 패널 -> 회원가입 성공 패널)
        signinButton.onClick.AddListener(OnSigninClicked);
        // 로그인 화면으로 가는 버튼 이벤트 연결 (회원가입 패널 -> 로그인 패널)
        goLoginPanelButton1.onClick.AddListener(OnLoginPanelClicked);
        // 로그인 화면으로 가는 버튼 이벤트 연결 (회원가입 성공 패널 -> 로그인 패널)
        goLoginPanelButton2.onClick.AddListener(OnLoginPanelClicked);
        // 옵션 세팅 버튼 이벤트 연결 (로그인 패널 -> 옵션 패널)
        optionButton.onClick.AddListener(OnOptionClicked);
        // 옵션 닫기 버튼 이벤트 연결 (옵션 패널 -> 로그인 패널)
        optionCloseButton.onClick.AddListener(OnOptionCloseClicked);
        // 게임종료 버튼 이벤트 연결
        exitButton.onClick.AddListener(OnExitClicked);
    }
    
    /// <summary>
    /// 로그인 유지 체크박스 처리 함수
    /// </summary>
    private void ToggleCheckBox()
    {
        isChecked = !isChecked;
        PlayerPrefs.SetInt("StayLoggedIn", isChecked ? 1 : 0);
        
        // 체크 해제 시 저장된 정보 초기화
        if (!isChecked)
        {
            PlayerPrefs.DeleteKey(SavedLoginIdKey);
            PlayerPrefs.DeleteKey(SavedLoginPwKey);
        }
        
        UpdateVisual();
    }
    
    private void UpdateVisual()
    {
        checkBoxMark.SetActive(isChecked);
    }
    
    private void OnLoginClicked()
    {
        string enteredId = loginIdField.text.Trim();
        string enteredPw = loginPasswordField.text.Trim();
        
        // 1. 임시 테스트 유저 목록에서 검사
        UserInfo user = testUserList.Find(u => u.Id == enteredId && u.Password == enteredPw);
        
        // 2. 회원가입한 임시 유저 목록에서 검사
        if (user == null)
        {
            user = signupUserList.Find(u => u.Id == enteredId && u.Password == enteredPw);
        }
        
        // 간단한 로그인 검증 (임시 테스트용 -> 리스트 사용)
        if (user != null)
        {
            // 로그인 성공 시 실패 문구 끄고 MainmenuScene으로 전환
            if (loginFailText != null) loginFailText.gameObject.SetActive(false);
            
            // 로그인 상태 유지 체크 시 저장
            if (isChecked)
            {
                PlayerPrefs.SetString(SavedLoginIdKey, enteredId);
                PlayerPrefs.SetString(SavedLoginPwKey, enteredPw);
            }
            
            SceneManager.LoadScene("MainmenuScene");
        }
        else
        {
            // 로그인 실패 시 문구 표시
            if (loginFailText != null) loginFailText.gameObject.SetActive(true);
        }
    }
    
    private void OnSignupClicked()
    {
        // 회원가입 패널 열기
        OpenPanel("[PopupPanel] Signup");
    }
    
    private void OnSigninClicked()
    {
        string userId = signupIdField.text.Trim();
        string userNickname = signupNickNameInputField.text.Trim();
        string userPassword = signupPasswordField.text.Trim();
        string userCheckPassword = signupCheckPasswordInputField.text.Trim();

        string errorMessage;

        if (TrySignUp(userId, userNickname, userPassword, userCheckPassword, out errorMessage))
        {
            // 회원가입 성공 시 유저 목록에 추가
            signupUserList.Add(new UserInfo(userId, userPassword, userNickname));
            
            // 회원가입 성공 시 실패 문구 끄고 [PopupPanel] SuccessSignup 열기
            if (signupFailText != null) signupFailText.gameObject.SetActive(false);
            OpenPanel("[PopupPanel] SuccessSignup");
        }
        else
        {
            // 회원가입 실패 시 문구 표시
            if (signupFailText != null)
            {
                signupFailText.text = errorMessage;
                signupFailText.gameObject.SetActive(true);
            }
        }
    }

    private void OnLoginPanelClicked()
    {
        OpenPanel("[PopupPanel] Login");
    }
    
    private void OnOptionClicked()
    {
        OpenPanel("[PopupPanel] Option");
    }

    private void OnOptionCloseClicked()
    {
        OpenPanel("[PopupPanel] Login");
    }
    
    private void OnExitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    #endregion

    
    #region 회원가입 에러 메세지 및 아이디 중복 확인

    private bool TrySignUp(string userId, string userNickname, string password, string checkPassword, out string errorMessage)
    {
        errorMessage = "";

        if (string.IsNullOrWhiteSpace(userId) 
            || string.IsNullOrWhiteSpace(userNickname)
            || string.IsNullOrWhiteSpace(password) 
            || string.IsNullOrWhiteSpace(checkPassword))
        {
            errorMessage = "모든 입력 칸을 채워주세요.";
            return false;
        }

        if (password != checkPassword)
        {
            errorMessage = "비밀번호가 일치하지 않습니다.";
            return false;
        }
        
        // 아이디 중복 확인 (테스트 유저 또는 기존 회원가입 유저)
        bool idExists =
            testUserList.Find(u => u.Id == userId) != null
            || signupUserList.Find(u => u.Id == userId) != null;
        
        if (idExists)
        {
            errorMessage = "이미 존재하는 아이디입니다.";
            return false;
        }
        
        return true;
    }

    #endregion
}
