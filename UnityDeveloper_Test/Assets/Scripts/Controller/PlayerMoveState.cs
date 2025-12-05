using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class PlayerMoveState : BaseState<PlayerController>
{
    [SerializeField] float _blendTime = 0.2f;
    [SerializeField] float _moveSpeed = 4.5f;
    [SerializeField] float _moveTimeScale = 50f;
    [SerializeField] float _roatationSpeed = 30f;
    Vector3 velocity;
    float time = 0;

    public PlayerMoveState(PlayerController m_Controller) : base(m_Controller)
    {
    }

    public void SetController(PlayerController m_Controller)
    {
        _controller = m_Controller;
    }

    public override void EnterState()
    {
        base.EnterState();
        if (_controller.OnGround && _controller.HasInput)
        {
            BlendMovementAsync();
        }
        SwitchState();
        Debug.Log("Enter MoveState");
    }

    public override void Update()
    {
        base.Update();
        SwitchState();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (_controller.OnGround)
        {
            if (_controller.HasInput)
            {
                CalculateMoveDirection();
                CalculateSpeed();
                Rotate();
                Move();
            }
            else
            {
                StopMoveMovement();
            }
        }
    }
  

    void SwitchState()
    {
        if (_controller.OnGround)
        {
            if (!_controller.HasInput)
            {
                _controller.SetState(_controller.State._idleState);
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
    }

    public async void BlendMovementAsync()
    {
        time = 0f;
        if (_controller.Animator.GetFloat("Idle_MovementBlend") == 1)
        {
            time = _blendTime;
        }

        while (time < _blendTime && _controller.HasInput)
        {
            time += Time.deltaTime;
            float lerpAnim = Mathf.Lerp(0f, 1f, time / _blendTime);
            _controller.Animator.SetFloat("Idle_MovementBlend", lerpAnim);
            await Task.Yield();
        }

        _controller.Animator.SetFloat("Idle_MovementBlend", 1f);
    }

    public override void ExitState()
    {
        base.ExitState();
        StopMoveMovement();
        Debug.Log("Exit MoveState");
    }

    private void CalculateMoveDirection()
    {
        Vector3 surfaceNormal = _controller._planeBeneathHit.normal;
        Vector3 newGlobaRight = Vector3.right;
        if (Mathf.Abs(Vector3.Dot(surfaceNormal,Vector3.up)) > 0.99f)
        {
            newGlobaRight = -Vector3.Cross(surfaceNormal, Vector3.forward).normalized;
        }
        else
        {
            newGlobaRight = Vector3.Cross(surfaceNormal, Vector3.up).normalized;
        }
        Vector3 newGlobalForward = Vector3.Cross(newGlobaRight, surfaceNormal);
        Vector3 inputVector = newGlobalForward * Input.GetAxis("Vertical") + newGlobaRight * Input.GetAxis("Horizontal");
        _controller.MoveDir = Vector3.ProjectOnPlane(inputVector, _controller._planeBeneathHit.normal).normalized;
    }

    private void CalculateSpeed()
    {
        Vector3 currentVel = _controller.RB.velocity;
        Vector3 targetVel = _controller.MoveDir * _moveSpeed;
        targetVel.y = currentVel.y;
        velocity = currentVel + (_controller.MoveDir * _moveSpeed - currentVel) * Time.deltaTime * _moveTimeScale;
        Debug.Log($"Velocity {velocity}");
    }

    void Move()
    {
        _controller.currentVelocity = _controller.RB.velocity = velocity;
    }

    void StopMoveMovement()
    {
        _controller.lastVelocity = _controller.currentVelocity;
        _controller.currentVelocity = Vector3.zero;

        _controller.LastMoveDir = _controller.MoveDir;
        _controller.MoveDir = Vector3.zero;

        _controller.RB.velocity = Vector3.zero;
        velocity = Vector3.zero;
    }

    public void Rotate()
    {
        if (_controller.MoveDir != Vector3.zero)
        {
            Vector3 lookDirection = _controller.MoveDir;
            Quaternion rotateTowards = Quaternion.LookRotation(lookDirection, _controller.transform.up);
            _controller.transform.localRotation = Quaternion.Slerp(_controller.transform.rotation, rotateTowards, _roatationSpeed * Time.deltaTime);
        }
    }
}
