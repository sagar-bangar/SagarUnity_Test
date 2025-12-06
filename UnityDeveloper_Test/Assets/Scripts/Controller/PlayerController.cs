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


    [Header("Movement Paremeters")]
    public Vector3 currentVelocity, lastVelocity;

    [SerializeField] Vector3 _moveDir;
    public Vector3 LastMoveDir { get; set; }
    public Vector3 MoveDir { get { return _moveDir; } set { _moveDir = value; } }


    [Header("GravitySwitch Parameters")]
    public Transform roataitonPiviot;
    public RaycastHit leftRayCast, rightRayCast, forwardRayCast, backRayCast;
    [SerializeField] RotationAxis currentRotationAxis;
    public RotationAxis CurrentRotationAxis { get => currentRotationAxis; set => currentRotationAxis = value; }

    public Camera _camera;

    #region MonoBehaviourCallbacks
    private void Awake()
    {
        _state = new PlayerStateFactory(this);
        _sphereCastHit = new RaycastHit();
        SetState(_state._idleState);
    }

    void Update()
    {
        UpdateMovemementBasedOnInput();
        UpdateRotationAxisBasedOnInput();
        UpdateJumpBasedOnInput();
        OnUpdateState();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        SetGroundedStatus();
        SetSlopeStatus();
        OnFixedUpdateState();
    }
    #endregion

    #region PhysicCast
    void ApplyGravity()
    {
        if (!OnGround)
        {
            _rb.AddForce(transform.up * -Physics.gravity.magnitude * _gravityScale, ForceMode.Acceleration);
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
    #endregion

    #region Input

    void UpdateMovemementBasedOnInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            SetInput(true);
        }
        else
        {
            SetInput(false);
        }

    }
    void SetInput(bool m_hasInput)
    {
        _hasInput = m_hasInput;
    }

    void UpdateJumpBasedOnInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetJump(true);
        }
        else
        {
            SetJump(false);
        }
    }

    void SetJump(bool m_hasJump)
    {
        _hasJumped = m_hasJump;
    }

    void UpdateRotationAxisBasedOnInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentRotationAxis = RotationAxis.Forward;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentRotationAxis = RotationAxis.Backward;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentRotationAxis = RotationAxis.Left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentRotationAxis = RotationAxis.Right;
        }
    }
    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        if (_planeBeneathHit.collider != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawRay(_planeBeneathHit.point, _planeBeneathHit.normal);

            Gizmos.DrawLine(transform.position, transform.position + LastMoveDir);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, (transform.position + MoveDir));
        }

        if (forwardRayCast.collider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(forwardRayCast.point, forwardRayCast.point + roataitonPiviot.up);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(forwardRayCast.point, forwardRayCast.point + forwardRayCast.normal);
        }
        Vector3 surfaceNormal = _planeBeneathHit.normal;

        Vector3 characterRightRelativeToLastMove = Vector3.Cross(surfaceNormal, LastMoveDir).normalized;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_planeBeneathHit.point, _planeBeneathHit.point + characterRightRelativeToLastMove);

    }
    #endregion
}
