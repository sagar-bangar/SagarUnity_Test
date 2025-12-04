using UnityEngine;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class PlayerJumpState : BaseState<PlayerController>
{
    [SerializeField] float jumpStrength = 4.5f;
    public PlayerJumpState(PlayerController m_Controller) : base(m_Controller)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        if(_controller.OnGround)
        {
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

        _controller.RB.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
    }

    public override void Update()
    {
        base.Update();
        SwitchState();
    }


    public override void ExitState()
    {
        base.ExitState();
        Debug.Log("Exit Jump State");
    }

    void SwitchState()
    {
       // _controller.SetState(_controller.State._fallState);
    }
}
