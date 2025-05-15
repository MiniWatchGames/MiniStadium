using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RunSkillState : PlayerActionState, ISkillData
{
    //이동 스킬 블루 프린트입니다
    private static int aniName;
    public RunSkillState() : base() { }
    private PassiveEffects effects;
    private GameObject effect;
    private bool _isNeedPresse;
    private float _skillMount;
    private ObjectPool _TrailObjectPool_Surface;
    private ObjectPool _TrailObjectPool_Joints;
    private GameObject _SurfacePrefab;
    private GameObject _JointsPrefab;

    public ObservableFloat CoolTime => null;
    public ObservableFloat NeedPressTime => null;
    public ObservableFloat PressTime => null;

    public bool IsNeedPresse => _isNeedPresse;
    public float SkillMount => _skillMount;

    private float _meshRegreshRate = 0.1f;
    private SkinnedMeshRenderer[] _skineedMeshRenderers;
    private Coroutine _runTrail;
    private string shaderVarRef = "_Alpha";
    private float shaderVarRate = 0.1f;
    private float shaderVarRefreshRate = 0.09f;
    [SerializeField] private float hueSpeed = 0.1f; // 증가당 hue 이동량

    private GameObject _trails;
    private WaitForSeconds waitTime;
    private int count = 0;

    private void Awake()
    {
        _JointsPrefab = Resources.Load<GameObject>("Prefabs/IngameScene/Player/MoveMentSkill/PlayerTrail_joints");
        _SurfacePrefab = Resources.Load<GameObject>("Prefabs/IngameScene/Player/MoveMentSkill/PlayerTrail_surface");
        _trails = new GameObject();
        _trails.name = "PlayerTrails";
        _TrailObjectPool_Surface = gameObject.AddComponent<ObjectPool>();
        _TrailObjectPool_Joints = gameObject.AddComponent<ObjectPool>();

        _TrailObjectPool_Surface.MakePool(_SurfacePrefab, 12, _trails.transform);
        _TrailObjectPool_Joints.MakePool(_JointsPrefab, 12, _trails.transform);

        _isNeedPresse = false;
        _skillMount = 1;
        waitTime = new WaitForSeconds(1f);
    }
    public override void Enter(PlayerController playerController)
    {
        //playerController.Animator.Play(aniName);
        base.Enter(playerController);

        if(effects == null)
        {
            effects = _playerController.GetComponent<PassiveEffects>();
        }if(effect == null)
        {
            Transform transform = _playerController.transform;
            if (_playerController.CameraController != null) {
                transform = _playerController.CameraController.transform;
            } 
                effect = Instantiate(effects.effect7, transform);
                effect.transform.localPosition = new Vector3(0, -0.5f, 2f);
            
        }
        else
        {
            effect.SetActive(true);
        }

        //값 지정 필요        
        _playerController.AddStatDecorate(StatType.MoveSpeed, _skillMount);
        _runTrail = _playerController.StartCoroutine(ActivateTrail());
    }
    public override void Exit()
    {
        effect?.SetActive(false);
        _playerController.RemoveStatDecorate(StatType.MoveSpeed);
        _playerController.StopCoroutine(_runTrail);
    }
    public override void StateUpdate()
    {
        base.StateUpdate();

    }

    public void ResetSkill() {
        Destroy(_TrailObjectPool_Surface);
        Destroy(_TrailObjectPool_Joints);
        Destroy(_trails);
        Destroy(effect);    
    }

    IEnumerator ActivateTrail()
    {
        while (true)
        {
            if (_skineedMeshRenderers == null)
            {
                _skineedMeshRenderers = _playerController.GetComponentsInChildren<SkinnedMeshRenderer>();
            }
            for (int i = 0; i < _skineedMeshRenderers.Length; i++)
            {
                GameObject obj = null;
                if (i == 0)
                {
                    obj = _TrailObjectPool_Surface.GetObject();
                }
                else {
                    obj = _TrailObjectPool_Joints.GetObject();
                }

                obj.transform.SetPositionAndRotation(_playerController.transform.position, _playerController.transform.rotation);
                MeshRenderer mr = obj.GetComponent<MeshRenderer>();
                MeshFilter mf = obj.GetComponent<MeshFilter>();
                Mesh mesh = new Mesh();
                _skineedMeshRenderers[i].BakeMesh(mesh);
                mf.mesh = mesh;
                float hue = Mathf.PingPong(count * hueSpeed, 0.5f) + 0.167f;
                Color color = Color.HSVToRGB(hue, 1f, 1f); // 형광 느낌: 채도=1, 명도=1
                mr.material.color = color;
                mr.material.SetFloat(shaderVarRef, 1);
                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));
                StartCoroutine(ReturnObj(i, obj));
            }
            count++;
            yield return new WaitForSeconds(_meshRegreshRate);
        }
    }

    IEnumerator ReturnObj(int i, GameObject obj)
    {
        yield return waitTime;
        if (i == 0)
        {
           _TrailObjectPool_Surface.ReturnObject(obj);
        }
        else
        {
           _TrailObjectPool_Joints.ReturnObject(obj);
        }
    }
    IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refrehRate)
    {
        float valueToAnimatie = mat.GetFloat(shaderVarRef);

        while (valueToAnimatie > goal)
        {
            valueToAnimatie -= rate;
            mat.SetFloat(shaderVarRef, valueToAnimatie);
            yield return new WaitForSeconds(refrehRate);
        }
    }
}