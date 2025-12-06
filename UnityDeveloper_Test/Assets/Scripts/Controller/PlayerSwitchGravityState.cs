using UnityEngine;

public class PlayerSwitchGravityState : BaseState<PlayerController>
{
    float _rotateSpeed = 20f;
    public PlayerSwitchGravityState(PlayerController m_Controller) : base(m_Controller)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Entered Switch Rotation");
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        switch (_controller.CurrentRotationAxis)
        {
            case RotationAxis.Forward:
                SetRaycastIndirection(Vector3.forward, _controller.forwardRayCast);
                break;
            case RotationAxis.Backward:
                SetRaycastIndirection(Vector3.back, _controller.backRayCast);
                break;
            case RotationAxis.Left:
                SetRaycastIndirection(Vector3.left, _controller.leftRayCast);
                break;
            case RotationAxis.Right:
                SetRaycastIndirection(Vector3.right, _controller.rightRayCast);
                break;
            default:
                SwitchState();
            break;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        _controller.CurrentRotationAxis = RotationAxis.Default;
        Debug.Log("Exit Switch Rotation");
    }

    void SetRaycastIndirection(Vector3 direction, RaycastHit raycast)
    {
        if (Physics.Raycast(_controller.roataitonPiviot.position, direction, out raycast, Mathf.Infinity, _controller._groundMask))
        {
            Vector3 currentUp = _controller.roataitonPiviot.up;
            Vector3 targetUp = raycast.normal;

            float dot = Vector3.Dot(currentUp, targetUp);
            if (dot > 0.999f)
            {
                SwitchState();
                return;
            }

            Quaternion alignRot = Quaternion.FromToRotation(currentUp, targetUp);
            Vector3 rotAxis = Vector3.Cross(currentUp, targetUp).normalized;
            Quaternion smoothed = Quaternion.Slerp(Quaternion.identity, alignRot, _rotateSpeed * Time.fixedDeltaTime);
            _controller.transform.RotateAround(_controller.roataitonPiviot.position, smoothed * rotAxis, smoothed.eulerAngles.magnitude);
        }
        else
        {
            SwitchState();
        }
    }

    void SwitchState()
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
        else
        {
            _controller.SetState(_controller.State._fallState);
        }
    }
}
