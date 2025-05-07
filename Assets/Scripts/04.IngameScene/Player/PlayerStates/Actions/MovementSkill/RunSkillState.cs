using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RunSkillState : PlayerActionState ,ISkillData
{
    //이동 스킬 블루 프린트입니다
    private static int aniName;
    public RunSkillState() : base() { }

    private bool _isNeedPresse;
    private float _skillMount;
    public ObservableFloat CoolTime => null;
    public ObservableFloat NeedPressTime => null;
    public ObservableFloat PressTime => null;

    public bool IsNeedPresse => _isNeedPresse;
    public float SkillMount => _skillMount;

    private float _meshRegreshRate = 0.1f;
    private SkinnedMeshRenderer[] _skineedMeshRenderers;
    private Coroutine _runTrail;
    private string shaderVarRef ="_Alpha";
    private float shaderVarRate = 0.1f;
    private float shaderVarRefreshRate = 0.09f;
    [SerializeField] private float hueSpeed = 0.1f; // 증가당 hue 이동량

    private int count = 0;

    private void Awake()
    {
        _isNeedPresse = false;
        _skillMount = 1;
    }

    public override void Enter(PlayerController playerController)
    {
        //playerController.Animator.Play(aniName);
        base.Enter(playerController);
        //값 지정 필요        
        _playerController.AddStatDecorate(StatType.MoveSpeed, _skillMount);
        _runTrail = _playerController.StartCoroutine(ActivateTrail());
    }
    public override void Exit()
    {
        _playerController.RemoveStatDecorate(StatType.MoveSpeed);
        _playerController.StopCoroutine(_runTrail);
        base.Exit();
    }
    public override void StateUpdate()
    {
        base.StateUpdate();
     
    }

    IEnumerator ActivateTrail() {
        while (true) { 
            if(_skineedMeshRenderers == null)
            {
                _skineedMeshRenderers = _playerController.GetComponentsInChildren<SkinnedMeshRenderer>();
            }
            for(int i = 0; i < _skineedMeshRenderers.Length; i++)
            {
                GameObject obj = new GameObject();
                obj.transform.SetPositionAndRotation(_playerController.transform.position,_playerController.transform.rotation);
                MeshRenderer mr =  obj.AddComponent<MeshRenderer>();
                MeshFilter mf = obj.AddComponent<MeshFilter>();
                Mesh mesh = new Mesh();
                _skineedMeshRenderers[i].BakeMesh(mesh);
                mf.mesh = mesh;
                mr.material = _playerController.RunStateMaterial;
                float hue = Mathf.PingPong(count * hueSpeed, 0.5f) + 0.167f;
                Color color = Color.HSVToRGB(hue, 1f, 1f); // 형광 느낌: 채도=1, 명도=1
                mr.material.color = color;
                StartCoroutine(AnimateMaterialFloat(mr.material, 0,shaderVarRate,shaderVarRefreshRate));
                Destroy(obj, 1f);
            }
            count++;
            yield return new WaitForSeconds(_meshRegreshRate);        
        }
    }

    IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refrehRate)
    {
        if (this == null || gameObject == null)
            yield break;

        float valueToAnimatie = mat.GetFloat(shaderVarRef);

        while (valueToAnimatie > goal) {
            valueToAnimatie -= rate;
            mat.SetFloat(shaderVarRef, valueToAnimatie);
            yield return new WaitForSeconds(refrehRate);
        }
    }
}