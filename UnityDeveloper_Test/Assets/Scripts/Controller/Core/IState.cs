public interface IState 
{
    public void EnterState();
    public void Update();
    public void FixedUpdate();
    public void ExitState();
}
