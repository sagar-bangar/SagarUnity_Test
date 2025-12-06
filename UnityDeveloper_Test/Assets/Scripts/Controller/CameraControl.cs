using UnityEngine;

public class CameraControl : MonoBehaviour
{

    // Reference
    public Transform _followTransform;
    [SerializeField] Camera _camera;
    [SerializeField] Vector3 _defaultCamPosition;
    [SerializeField] LayerMask _obstacleMask;
    // Parameters
    public float _smoothTime = 0.3f;
    [SerializeField] private float _rotationDamping;
    private Vector3 _velocity = Vector3.zero;
    public float sensitivity;

    [Header("SphereCasr Parameters")]
    RaycastHit _playerCamerRacast;
    [SerializeField] float _sphereRadius,_collosingSmothTime;
    [SerializeField] Transform _headPoint, _footPoint;
    [SerializeField] float cameraCollosionOffset;

    void Start()
    {
        _camera.transform.localPosition = _defaultCamPosition;
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        HandleCameraCollision();
    }

    void LateUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleCameraCollision()
    {
        Vector3 desiredCamPos = transform.position + transform.right * _defaultCamPosition.x + transform.up * _defaultCamPosition.y + transform.forward * _defaultCamPosition.z;

        Vector3 centerPoint = (_headPoint.position + _footPoint.position) * 0.5f;
        Vector3 direction = desiredCamPos - centerPoint;
        float maxDist = direction.magnitude;

        Vector3 targetPos;

        if (Physics.SphereCast(centerPoint, _sphereRadius, direction.normalized, out _playerCamerRacast, maxDist, _obstacleMask))
        {
            targetPos = _playerCamerRacast.point - direction.normalized * cameraCollosionOffset;

        }
        else
        {
            targetPos = desiredCamPos;
        }
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, targetPos, Time.deltaTime * _collosingSmothTime);

    }



    private void HandleMovement()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _followTransform.position, ref _velocity, _smoothTime);
    }

    float xRot;
    float yRot;
    private void HandleRotation ()
    {
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            xRot += mouseX;
            yRot -= mouseY;
            yRot = Mathf.Clamp(yRot, -45f, 45f);

            Quaternion targetRot = Quaternion.Euler(yRot, xRot, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotationDamping * Time.deltaTime);
        }
    }
}