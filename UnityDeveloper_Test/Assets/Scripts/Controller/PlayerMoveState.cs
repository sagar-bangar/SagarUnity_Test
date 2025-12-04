using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class PlayerMoveState : BaseState<PlayerController>
{
    [SerializeField] float _blendTime = 0.2f;
    [SerializeField] float _moveSpeed = 4.5f;
    [SerializeField] float _maxMoveSpeed = 50f;
    [SerializeField] float _roatationSpeed = 30f;
    Vector3 velocity;
    float time = 0;

    public PlayerMoveState(PlayerController m_Controller) : base(m_Controller)
    {
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

    private void CalculateMoveDirection()
    {
        float time = 0;
        time += Time.deltaTime;
        Vector3 targetDirection = Vector3.zero;
        Vector3 averageDirection = Vector3.zero;
        targetDirection = (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))).normalized;
        float desiredDirection = Vector3.Dot(_controller.transform.up, _controller._planeBeneathHit.normal);
        float angle = Mathf.Acos(desiredDirection) * Mathf.Rad2Deg;
        Vector3 rotAxis = Vector3.Cross(_controller.transform.up, _controller._planeBeneathHit.normal);
        Quaternion rot = Quaternion.AngleAxis(angle, rotAxis);
        targetDirection = rot * targetDirection;
        _controller.MoveDir = targetDirection;
    }

    void SwitchState()
    {
        if (_controller.OnGround)
        {
            if (!_controller.HasInput)
            {
                _controller.SetState(_controller.State._idleState);
            }
        }
        /*else
        {
            _controller.SetState(_controller.State._fallState);
        }*/
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

    private void CalculateSpeed()
    {
        float accel = _maxMoveSpeed;
        Vector3 currentVel = _controller.RB.velocity;

        Vector3 targetVel = _controller.MoveDir * _moveSpeed;
        targetVel.y = currentVel.y;

        Vector3 delta = targetVel - currentVel;
        Vector3 accelStep = delta.normalized * accel * Time.deltaTime;
        if (accelStep.magnitude > delta.magnitude)
            accelStep = delta;

        velocity = currentVel + accelStep;

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
            Vector3 lookDirection = new Vector3(_controller.MoveDir.x, 0, _controller.MoveDir.z);
            Quaternion rotateTowards = Quaternion.LookRotation(lookDirection, Vector3.up);
            _controller.transform.localRotation = Quaternion.Slerp(_controller.transform.rotation, rotateTowards, _roatationSpeed * Time.deltaTime);
        }
    }
}
