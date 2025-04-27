using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using UnityEngine;

public class Attack : NetworkBehaviour
{
    public Weapon weapon;
    
    public override void OnStartNetwork()
    {
        TimeManager.OnTick += OnTick;
        TimeManager.OnPostTick += OnPostTick;
    }
    public override void OnStopNetwork()
    {
        TimeManager.OnTick -= OnTick;
        TimeManager.OnPostTick -= OnPostTick;
    }

    private void OnTick() => DoAttack(CreateAttackData());
    private void OnPostTick() => CreateReconcile();

    public override void CreateReconcile() =>ReconcileState(new Data.AttackReconcileData());

    private Data.AttackData CreateAttackData()
    {
        if (!IsOwner)
        {
            return default;
        }

        return new Data.AttackData(Input.GetKey(KeyCode.Mouse0));
    }

    [Replicate]
    private void DoAttack(Data.AttackData data, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
    {
        if (data.Attack)
        {
            weapon.Attack();
        }
    }
    
    [Reconcile]
    private void ReconcileState(Data.AttackReconcileData data, Channel channel = Channel.Unreliable) { }
}