using System.Collections;
using System.Collections.Generic;
using FishNet.Demo.Prediction.Rigidbodies;
using FishNet.Object.Prediction;
using UnityEngine;

public class Data : MonoBehaviour
{
    public struct AttackData : IReplicateData
    {
        public bool Attack;

        public AttackData(bool attack) : this()
        {
            Attack = attack;
        }

        private uint _tick;
        public void Dispose() { }
        public uint GetTick() { return _tick; }
        public void SetTick(uint value) { _tick = value; }
    }
    
    public struct AttackReconcileData : IReconcileData
    {

        public bool Attack;

        public AttackReconcileData(bool attack) : this()
        {
            Attack = attack;
        }

        private uint _tick;
        public void Dispose(){ }

        public uint GetTick()
        {
            return _tick;
        }

        public void SetTick(uint value)
        {
            _tick = value;
        }
    }
    
    public struct ReplicateData : IReplicateData
    {
        public bool Jump;
        public float Horizontal;
        public float Vertical;

        public float Yaw;
        public float Pitch;

        public bool Run;

        public ReplicateData(bool jump, float horizontal, float vertical, float yaw, float pitch, bool run) : this()
        {
            Jump = jump;
            Horizontal = horizontal;
            Vertical = vertical;
            
            Yaw = yaw;
            Pitch = pitch;
            
            Run = run;
        }

        private uint _tick;
        public void Dispose(){}
        public uint GetTick() => _tick;
        public void SetTick(uint vlaue) => _tick = vlaue;
    }

    public struct ReconcileData : IReconcileData
    {

        public PredictionRigidbody PredictionRigidbody;

        public ReconcileData(PredictionRigidbody pr) : this()
        {
            PredictionRigidbody = pr;
        }

        private uint _tick;
        public void Dispose(){ }

        public uint GetTick()
        {
            return _tick;
        }

        public void SetTick(uint value)
        {
            _tick = value;
        }
    }
}
