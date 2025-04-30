using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]

public class PlayerControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //기본 인스펙터 를 그리기
        base.OnInspectorGUI();

        //타겟  컴포넌트 참조 가져오기
        PlayerController playerController = (PlayerController)target;


        // 여백 추가
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Action 상태 디버그 정보",EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        if(playerController?.ActionFsm != null)
        {

            switch (playerController.ActionFsm.CurrentState)
            {
                case ActionState.Idle:
                    GUI.backgroundColor = new Color(1, 1, 1, 1);
                    break;
                case ActionState.Hit:
                    GUI.backgroundColor = new Color(1, 0, 0, 1);
                    break;
                case ActionState.Dead:
                    GUI.backgroundColor = new Color(0, 1, 0, 1);
                    break;
                case ActionState.Attack:
                    GUI.backgroundColor = new Color(0, 0, 1, 1);
                    break;
                case ActionState.MovementSkills:
                    GUI.backgroundColor = new Color(1, 1, 0, 1);
                    break;
                case ActionState.WeaponSkills:
                    GUI.backgroundColor = new Color(0, 1, 1, 1);
                    break;
                case ActionState.None:
                    GUI.backgroundColor = new Color(1, 0, 1, 1);
                    break;
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.LabelField("현재 상태", playerController?.ActionFsm?.CurrentState.ToString(), EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        // 여백 추가
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Posture 상태 디버그 정보", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        if (playerController?.PostureFsm != null)
        {
            switch (playerController.PostureFsm.CurrentState)
            {
                case PostureState.Idle:
                    GUI.backgroundColor = new Color(1, 1, 1, 1);
                    break;
                case PostureState.Crouch:
                    GUI.backgroundColor = new Color(1, 0, 0, 1);
                    break;
                case PostureState.None:
                    GUI.backgroundColor = new Color(0, 1, 0, 1);
                    break;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.LabelField("현재 상태", playerController?.PostureFsm?.CurrentState.ToString(), EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        // 여백 추가
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Movement 상태 디버그 정보", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        if (playerController?.MovementFsm != null)
        {
            switch (playerController.MovementFsm.CurrentState)
            {
                case MovementState.Idle:
                    GUI.backgroundColor = new Color(1, 1, 1, 1);
                    break;
                case MovementState.Jump:
                    GUI.backgroundColor = new Color(1, 0, 0, 1);
                    break;
                case MovementState.Walk:
                    GUI.backgroundColor = new Color(0, 1, 0, 1);
                    break;
                case MovementState.None:
                    GUI.backgroundColor = new Color(1, 0, 1, 1);
                    break;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.LabelField("현재 상태", playerController?.MovementFsm?.CurrentState.ToString(), EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();



    }

    private void OnEnable()
    {
       EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate() {
        // 플레이 모드일 때만 인스펙터 갱신
        if (!Application.isPlaying)
            return;

        if (target != null)
        {
            Repaint();
        }
    }
}
