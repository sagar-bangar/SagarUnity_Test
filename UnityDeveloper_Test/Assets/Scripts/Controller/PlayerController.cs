using UnityEngine;

public class PlayerController : BaseController
{
    [SerializeField] bool _hasInput, _hasJumped, _onGround, _onSlope;
    public bool HasInput => _hasInput;
    public bool OnGround => _onGround;
    public bool HasJumped => _hasJumped;

    [SerializeField] PlayerStateFactory _state;
    public PlayerStateFactory State => _state;

    [Header("Physics References")]
    [SerializeField] CapsuleCollider _capsuleCollider;
    [SerializeField] Rigidbody _rb;
    public Rigidbody RB => _rb;

    [Header("GroundCheck Parameter")]
    private RaycastHit _sphereCastHit;
    public float _sphereCastDistance, _sphereRadiusScale, _spherCastStart;
    public LayerMask _groundMask;

    [Header("PlaneCheck Parameter")]
    public RaycastHit _planeBeneathHit;
    [SerializeField] float _planeCheckRayDistance;

    [SerializeField] Animator _animator;
    public Animator Animator => _animator;

    [SerializeField] float _gravityScale;

    public Vector3 currentVelocity, lastVelocity;

    [SerializeField] Vector3 _moveDir;
    public Vector3 MoveDir { get { return _moveDir; } set { _moveDir = value; } }

    [SerializeField] Vector3 _lastMoveDir;
    public Vector3 LastMoveDir { get { return _lastMoveDir; } set { _lastMoveDir = value; } }

    public Transform roataitonPiviot;
    public RaycastHit leftRayCast, rightRayCast, forwardRayCast, backRayCast;

    [SerializeField] RotationAxis currentRotationAxis;
    public RotationAxis CurrentRotationAxis { get => currentRotationAxis; set => currentRotationAxis = value; }

    private void Awake()
    {
        _state = new PlayerStateFactory(this);
        _sphereCastHit = new RaycastHit();
        SetState(_state._idleState);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            SetInput(true);
        }
        else
        {
            SetInput(false);
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentRotationAxis = RotationAxis.Forward;
            SetState(State._switchGravityState);
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentRotationAxis = RotationAxis.Backward;
            SetState(State._switchGravityState);
        }
        else if( Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentRotationAxis = RotationAxis.Left;
            SetState(State._switchGravityState);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentRotationAxis = RotationAxis.Right;
            SetState(State._switchGravityState);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetJump(true);
        }
        else
        {
            SetJump(false);
        }
        OnUpdateState();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        SetGroundedStatus();
        SetSlopeStatus();
        OnFixedUpdateState();

    }

    void ApplyGravity()
    {
        if (!OnGround)
        {
            _rb.AddForce(transform.up * -Physics.gravity.magnitude * _gravityScale ,ForceMode.Acceleration);
        }
    }

    void SetGroundedStatus()
    {
        if (Physics.SphereCast(transform.position + (transform.up * _spherCastStart), _capsuleCollider.radius * _sphereRadiusScale, -transform.up, out _sphereCastHit, _sphereCastDistance, _groundMask))
        {
            _onGround = true;
        }
        else
        {
            _onGround = false;
        }
    }

    void SetSlopeStatus()
    {
        if (Physics.Raycast(transform.position, -transform.up, out _planeBeneathHit, _planeCheckRayDistance, _groundMask))
        {
            if (_planeBeneathHit.normal == transform.up)
            {
                _onSlope = false;
            }
            else
            {
                _onSlope = true;
            }
        }
    }




    void SetInput(bool m_hasInput)
    {
        _hasInput = m_hasInput;
    }

    void SetJump(bool m_hasJump)
    {
        _hasJumped = m_hasJump;
    }

    private void OnDrawGizmos()
    {
        if (_planeBeneathHit.collider != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawRay(_planeBeneathHit.point, _planeBeneathHit.normal);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, (transform.position + MoveDir));
        }

        if(forwardRayCast.collider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(forwardRayCast.point, forwardRayCast.point + roataitonPiviot.up);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(forwardRayCast.point, forwardRayCast.point + forwardRayCast.normal);
        }
    }
}
