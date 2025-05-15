using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAttacking : MonoBehaviour
{
    public Action<InGameManager.RoundState> StateChange;
    private Coroutine Active;
    [SerializeField] private PlayerController dummy;
    Vector2[] directions = new Vector2[]
              {
                        new Vector2(0, 1),
                        new Vector2(1, 1),
                        new Vector2(1, 0),
                        new Vector2(1, -1),
                        new Vector2(0, -1),
                        new Vector2(-1, -1),
                        new Vector2(-1, 0),
                        new Vector2(-1, 1),
              };

    (int, string)[] skills = new (int, string)[]
    {
        (1,"달리기"),
        (0,""),
        (0,""),
    };
    // Start is called before the first frame update
    private void Start()
    {
        StateChange = (state) =>
        {
            switch (state)
            {
                case InGameManager.RoundState.RoundEnd:
                    if (Active != null) { 
                        StopCoroutine(Active);
                        Active = null;
                    }
                    break;

                case InGameManager.RoundState.InRound:
                    if (Active == null)
                    {
                        Active = StartCoroutine(DoSomething());
                    }
                    break;
                case InGameManager.RoundState.SuddenDeath:
                    if (Active == null) { 
                        Active = StartCoroutine(DoSomething());
                    }
                    break;
            }
        };
        dummy.ActionFsm?.AddSkillState(dummy, skills, 0);
    }

    
    private int GetRandomnumber(int size)
    {
        int baseRandom = UnityEngine.Random.Range(0, 1000);
        System.Random subRand = new System.Random(baseRandom);
        int subRandom = subRand.Next(0, size);
        return subRandom;
    }

    IEnumerator DoSomething()
    {
        while (dummy.ActionFsm.CurrentState != ActionState.Dead)
        {
            yield return new WaitForSeconds(3f);

            int walkingNum = GetRandomnumber(3);
            switch (walkingNum)
            {
                case 0:
                    dummy.SetMovementState("Idle");

                    break;
                case 1:
                    walk();
                    break;
                case 2:
                    walk();
                    dummy.SetActionState("RunSkill");
                    yield return new WaitForSeconds(2f);
                    break;
            }

            int somethingNum = GetRandomnumber(3);
            switch (somethingNum)
            {
                case 0:
                    dummy.SetActionState("Idle");
                    break;
                case 1:
                    dummy.SetActionState("Attack");
                    break;
                case 2:
                    dummy.SetActionState("Attack");
                    yield return new WaitForSeconds(0.6f);
                    var sc = ((SwordAttackStrategy)dummy.CombatManager.AttackStrategies[WeaponType.Sword]);
                    sc.ComboActive = true;
                    sc.HandleInput(true, true);
                    yield return new WaitForSeconds(0.1f);
                    sc.ComboActive = false;
                    break;
            }
        }
    }

    private void walk() {
        int walkingDirection = GetRandomnumber(7);
        Vector3 selected3D = new Vector3(directions[walkingDirection].x, 0, directions[walkingDirection].y);
        transform.rotation = Quaternion.LookRotation(selected3D);
        dummy.CurrentMoveInput = directions[walkingDirection];
        dummy.SetMovementState("Walk");
    }

}
