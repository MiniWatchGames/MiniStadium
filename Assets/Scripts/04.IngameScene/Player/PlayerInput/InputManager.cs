using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    private IInputEvents _listener;
    
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    // Input Listener 구독
    public void Register(IInputEvents listener)
    {
        _listener = listener;
    }
    
    // 구독 해제 
    public void Unregister(IInputEvents listener)
    {
        if (_listener == listener)
        {
            _listener = null;
        }
    }

    private void Update()
    {
        MoveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        LookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        
        _listener?.OnMove(MoveInput);
        //_listener?.OnLook(LookInput);

        if (Input.GetButtonDown("Jump")) _listener?.OnJumpPressed();
        if (Input.GetButtonDown("Fire1")) _listener?.OnFirePressed();
        if (Input.GetButtonUp("Fire1")) _listener?.OnFireReleased();
        if (Input.GetKeyDown(KeyCode.C)) _listener?.OnCrouchPressed();
        if (Input.GetButtonDown("FirstWeaponSkill")) _listener?.OnFirstWeaponSkillPressed();
        if (Input.GetButtonUp("FirstWeaponSkill")) _listener?.OnFirstWeaponSkillReleased();
        if (Input.GetButtonDown("SecondWeaponSkill")) _listener?.OnSecondWeaponSkillPressed();
        if (Input.GetButtonUp("SecondWeaponSkill")) _listener?.OnSecondWeaponSkillReleased();
        if (Input.GetButtonDown("FirstMoveSkill")) _listener?.OnFirstMoveSkillPressed();
        if (Input.GetButtonUp("FirstMoveSkill")) _listener?.OnFirstMoveSkillReleased();
        if (Input.GetButtonDown("SecondMoveSkill")) _listener?.OnSecondMoveSkillPressed();
        if (Input.GetButtonUp("SecondMoveSkill")) _listener?.OnSecondMoveSkillReleased();
    }
}
