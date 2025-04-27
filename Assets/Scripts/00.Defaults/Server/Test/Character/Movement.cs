using System.Collections;
using System.Collections.Generic;
using FishNet.Demo.Prediction.Rigidbodies;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities;
using Unity.VisualScripting;
using UnityEngine;


public class Movement : NetworkBehaviour
{
    public const float RunSpeed = 6f;
    public const float WalkSpeed = 3f;
    public const float GroundDrag = 5f;
    public const float AirDrag = 1f;

    public const float AirMoveForceRate = 0.5f;
    [SerializeField] private float JumpForce = 8f;
    [SerializeField] private float MoveForce = 15f;

    [SerializeField] private Transform yawTransform;
    [SerializeField] private Transform pitchTransform;

    private bool _jump;
    private float _pitch;
    private float _yaw;
    
    private PredictionRigidbody predictionRigidbody;
    private Data.ReplicateData previousData;

    private float Pitch
    {
        get => _pitch;
        set
        {
            _pitch = value;
            if (_pitch > 90f) _pitch = 90f;
            else if (_pitch < -90f) _pitch = -90f;
        }
    }

    private float Yaw
    {
        get  => _yaw;
        set
        {
            _yaw = value;
            if(_yaw > 180) _yaw -= 360;
            else if (_yaw < -180) _yaw += 360;
        }
    }

    private void Awake()
    {
        predictionRigidbody = ObjectCaches<PredictionRigidbody>.Retrieve();
        predictionRigidbody.Initialize(GetComponent<Rigidbody>());
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jump = true;
        }

        Yaw += Input.GetAxis("Mouse X");
        Pitch -= Input.GetAxis("Mouse Y");
    }

    private void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        yawTransform.localEulerAngles = new Vector3(0f, Yaw, 0f);
        pitchTransform.localEulerAngles = new Vector3(Pitch, 0f, 0f);
    }

    private void OnDestroy()
    {
        ObjectCaches<PredictionRigidbody>.StoreAndDefault(ref predictionRigidbody);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f);
    }

    public override void OnStartNetwork()
    {
        TimeManager.OnTick += TimeManager_OnTick;
        TimeManager.OnPostTick += TimeManager_OnPostTick;
    }
    public override void OnStopNetwork()
    {
        TimeManager.OnTick -= TimeManager_OnTick;
        TimeManager.OnPostTick -= TimeManager_OnPostTick;
    }

    private void TimeManager_OnTick()
    {
        RunInputs(CreateReplicateData());
    }

    private Data.ReplicateData CreateReplicateData()
    {
        if (!IsOwner)
        {
            return default;
        }
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var run = Input.GetKey(KeyCode.LeftShift);
        var md = new Data.ReplicateData(_jump, horizontal, vertical, Yaw, Pitch, run);
        _jump = false;
        return md;
    }

    [Replicate]
    private void RunInputs(Data.ReplicateData data, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
    {
        previousData = data;
        var isGround = IsGrounded();
        
        predictionRigidbody.Rigidbody.drag = isGround ? GroundDrag : AirDrag;
        
        var moveDir = Quaternion.Euler(0f, data.Yaw, 0f) * new Vector3(data.Horizontal, 0f, data.Vertical);
        moveDir.Normalize();
        
        var moveForce = isGround ? 15 : 15 * AirMoveForceRate;
        
        predictionRigidbody.Rigidbody.AddForce(moveDir * moveForce, ForceMode.Acceleration);

        var jump = isGround && data.Jump;
        
        if(jump)
        {
            var jumpForce = new Vector3(0f, JumpForce, 0f);
            predictionRigidbody.Rigidbody.AddForce(jumpForce, ForceMode.Impulse);
        }
        
        yawTransform.localEulerAngles = new Vector3(0f, data.Yaw, 0f);
        pitchTransform.localEulerAngles = new Vector3(data.Pitch, 0f, 0f);
        
        predictionRigidbody.Simulate();
    }

    private void TimeManager_OnPostTick()
    {
        if (HasAuthority)
        {
            LimitVelocity(previousData);
        }

        CreateReconcile();
    }

    private void LimitVelocity(Data.ReplicateData data)
    {
        var currentVelocity = predictionRigidbody.Rigidbody.velocity;
        var horizontalVelocity = new Vector2(currentVelocity.x, currentVelocity.z);
        var maxSpeed = data.Run ? RunSpeed : WalkSpeed;
        var isOver = horizontalVelocity.magnitude > maxSpeed;

        horizontalVelocity = isOver ? horizontalVelocity.normalized * maxSpeed : horizontalVelocity;
        if (isOver)
        {
            predictionRigidbody.Velocity(new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.y));
        }
    }

    public override void CreateReconcile()
    {
        ReconcileState(new Data.ReconcileData(predictionRigidbody));
    }

    [Reconcile]
    private void ReconcileState(Data.ReconcileData data, Channel channel = Channel.Unreliable)
    {
        predictionRigidbody.Reconcile(data.PredictionRigidbody);
    }
}
