public interface IAttackStrategy
{
    public void Enter(PlayerController playerController);
    public void Update(PlayerController playerController);
    public void Exit();
    public void HandleInput(bool isFirePressed, bool isFireHeld);
    public bool IsComplete();
}
