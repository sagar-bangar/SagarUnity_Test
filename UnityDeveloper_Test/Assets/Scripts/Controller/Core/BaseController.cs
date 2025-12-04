using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    protected IState currentState;

    public void SetState(IState newState)
    {
        if (currentState != null)
        {
            if (currentState.Equals(newState))
            {
                return;
            }
            currentState.ExitState();
        }
        currentState = newState;
        currentState.EnterState();
    }

    protected void OnUpdateState()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    protected void OnFixedUpdateState()
    {
        if(currentState != null)
        {
            currentState.FixedUpdate();
        }
    }
}
