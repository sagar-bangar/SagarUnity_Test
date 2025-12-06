using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class PlayerIdleState : BaseState<PlayerController>
{
    [SerializeField] float _blendTime = 0.2f;
    float time = 0;
    public PlayerIdleState(PlayerController m_Controller) : base(m_Controller)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        if (_controller.OnGround)
        {
            if (!_controller.HasInput)
            {
                _controller.RB.velocity = Vector3.zero;
                BlendMovementAsync();
            }
        }
        Debug.Log("Entered Idle State");
    }

    public async void BlendMovementAsync()
    {
        time = 0f;
        if (_controller.Animator.GetFloat("Idle_MovementBlend") == 0)
        {
            time = _blendTime;
        }

        while (time < _blendTime && !_controller.HasInput)
        {
            time += Time.deltaTime;
            float lerpAnim = Mathf.Lerp(1f, 0f, time / _blendTime);
            _controller.Animator.SetFloat("Idle_MovementBlend", lerpAnim);
            await Task.Yield();
        }

        _controller.Animator.SetFloat("Idle_MovementBlend", 0f);
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
            else if(_controller.HasJumped)
            {
                _controller.SetState(_controller.State._jumpState);
            }
        }
        else
        {
            _controller.SetState(_controller.State._fallState);
        }

        if (_controller.CurrentRotationAxis != default)
        {
            _controller.SetState(_controller.State._switchGravityState);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        Debug.Log("Exit Idle State");
    }
}
