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
        if (Physics.SphereCast(transform.position + (Vector3.up * _spherCastStart), _capsuleCollider.radius * _sphereRadiusScale, -transform.up, out _sphereCastHit, _sphereCastDistance, _groundMask))
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
        if (Physics.Raycast(transform.position, Vector3.down, out _planeBeneathHit, _planeCheckRayDistance, _groundMask))
        {
            if (_planeBeneathHit.normal == Vector3.up)
            {
                _onSlope = true;
            }
            else
            {
                _onSlope = false;
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

    private void OnDrawGizmosSelected()
    {
        if (_capsuleCollider == null) return;

        Gizmos.color = Color.red;

        float radius = _capsuleCollider.radius * _sphereRadiusScale;

        Vector3 start = transform.position + (Vector3.up * _spherCastStart);
        Vector3 end = start + (-transform.up * _sphereCastDistance);

        Gizmos.DrawWireSphere(start, radius);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(end, radius);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, end);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * _planeCheckRayDistance));
    }
}
