using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFallState : BaseState<PlayerController>
{
    public PlayerFallState(PlayerController m_Controller) : base(m_Controller)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Entered Fall State");
        if (!_controller.OnGround)
        {
            _controller.Animator.SetBool("IsFalling", true);
        }
    }

    public override void Update()
    {
        base.Update();
        SwitchState();
    }

    public void SwitchState()
    {
        if (_controller.OnGround)
        {
            if (_controller.HasInput)
            {
                _controller.SetState(_controller.State._moveState);
            }
            else
            {
                _controller.SetState(_controller.State._idleState);
            }
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        Debug.Log("Exit Fall State");
        _controller.Animator.SetBool("IsFalling", false);
    }
}
