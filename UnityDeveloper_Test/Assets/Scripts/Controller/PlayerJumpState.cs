using UnityEngine;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class PlayerJumpState : BaseState<PlayerController>
{
    [SerializeField] float jumpStrength = 4.5f;
    bool hasJumped;
    public PlayerJumpState(PlayerController m_Controller) : base(m_Controller)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        if(_controller.OnGround)
        {
            hasJumped = true;
            Jump();
        }
        Debug.Log("Enter Jump State");
    }

    private void Jump()
    {
        Debug.Log("Jumping");
        Vector3 vel = Vector3.zero;
        if (_controller.HasInput)
        {
            vel = _controller.LastMoveDir * _controller.lastVelocity.magnitude;
        }
        vel.y = 0;
        _controller.RB.velocity = vel;

        _controller.RB.AddForce(_controller.transform.up * jumpStrength, ForceMode.Impulse);
    }

    public override void Update()
    {
        SwitchState();
        base.Update();
    }


    public override void ExitState()
    {
        base.ExitState();
        Debug.Log("Exit Jump State");
    }

    void SwitchState()
    {
        if(!_controller.OnGround)
        {
            _controller.SetState(_controller.State._fallState);
        }
    }
}
