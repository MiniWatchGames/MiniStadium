using Unity.Services.Matchmaker.Models;
using UnityEngine;

public interface IAttackStrategy
{
    public void Enter(PlayerController playerController, GameObject weaponObject);
    public void Update(PlayerController playerController);
    public void Exit();
    public void HandleInput(bool isFirePressed, bool isFireHeld);
    public bool IsComplete();
}

public interface IWeaponSkillStrategy
{
    public void Enter(PlayerController playerController, GameObject weaponObject);
    public void Update(PlayerController playerController);
    public void Exit();
    public void HandleInput(bool isSkillPressed, bool isSkillHeld);
    public bool IsComplete();
}