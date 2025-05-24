using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Core Settings")]
    [SerializeField] private PlayerMotorSettings _settings;
    [SerializeField] private Transform _camera;
    [SerializeField] private bool _debug;

    [Header("Reset Settings")]
    [Tooltip("Y-position threshold below which the player resets")]
    [SerializeField] private float fallResetHeight = -10f;
    [Tooltip("Y-position the player is moved to upon reset")]
    [SerializeField] private float resetPositionHeight = 10f;

    [Header("Gravity & Jump Settings")]
    [Tooltip("Downward force to stick to the ground when grounded")]
    [SerializeField] private float stickyGroundForce = 0.5f;

    [Header("Ski Mode Settings")]
    [Tooltip("Mouse button index to hold for skiing (0=left, 1=right, etc.)")]
    [SerializeField] private int skiMouseButton = 1;
    [Tooltip("Speed multiplier applied when skiing")]
    [SerializeField] private float skiSpeedMultiplier = 1.5f;
    [Tooltip("Friction coefficient when skiing (typically zero)")]
    [SerializeField] private float skiFrictionCoefficient = 0f;

    [Header("Air & Slope Settings")]
    [Tooltip("Multiplier for movement when airborne")]
    [SerializeField] private float airMovementMultiplier = 0.1f;
    [Tooltip("Factor used to scale slope influence")]
    [SerializeField] private float slopeBoostFactor = 0.00278f;

    private CharacterController _motor;
    private float _cameraRotX;
    private bool _jumpKeyPressed;
    private bool _playerGrounded;
    private Vector3 _velocity;
    private Vector3 _lastVelocity;
    private RaycastHit _downHit;
    private (Vector3 dir, float angle) _slope;
    private float _verticalVelocity;

    private void Awake()
    {
        _motor = GetComponent<CharacterController>();
        if (_settings.lockCursorOnAwake)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        HandleFallReset();
        SampleSlope();
        HandleInputs();
        HandleGravityAndJump();
        HandlePosition();
        HandleCameraAndBodyRotation();
    }

    private void HandleFallReset()
    {
        if (transform.position.y < fallResetHeight)
        {
            _motor.enabled = false;
            transform.position = Vector3.up * resetPositionHeight;
            _motor.enabled = true;
        }
    }

    private void SampleSlope()
    {
        _slope = SlopeDirection();
        Physics.SphereCast(
            transform.position,
            _settings.groundRaycastRadius,
            Vector3.down,
            out _downHit,
            _settings.groundRaycastDistance,
            ~LayerMask.GetMask("Player")
        );
        _playerGrounded = _motor.isGrounded;
    }

    private void HandleInputs()
    {
        if (_settings.canJump && _playerGrounded && Input.GetKeyDown(_settings.jumpKey))
            _jumpKeyPressed = true;
    }

    private void HandleGravityAndJump()
    {
        if (_playerGrounded)
        {
            // Sticky force for ground contact
            _verticalVelocity = -stickyGroundForce;
            if (_jumpKeyPressed)
            {
                _verticalVelocity = _settings.jumpForce;
                _jumpKeyPressed = false;
            }
        }
        else
        {
            // Apply gravity when in air
            _verticalVelocity -= _settings.gravity * Time.deltaTime;
        }
    }

    private void HandlePosition()
    {
        bool skiing = Input.GetMouseButton(skiMouseButton) && _playerGrounded;
        float speedMod = skiing ? skiSpeedMultiplier : 1f;

        // Input direction
        var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        input = Vector3.ClampMagnitude(input, 1);
        input = transform.TransformDirection(AdjustedMovementAcceleration(input));

        // Movement delta (ground vs air)
        var movementDelta = _playerGrounded
            ? input * speedMod * Time.deltaTime
            : input * speedMod * airMovementMultiplier * Time.deltaTime;

        // Slope influence
        var slopeDelta = (_slope.angle > _settings.minAngleForSlope)
            ? _slope.dir * (_slope.angle * slopeBoostFactor * _settings.slopeAcceleration * Time.deltaTime)
            : Vector3.zero;

        // Friction
        float frictionCoef = !_playerGrounded
            ? _settings.frictionInAir
            : (skiing ? skiFrictionCoefficient : _settings.frictionOnGround);
        var frictionDelta = -_lastVelocity / Time.deltaTime * (frictionCoef * Time.deltaTime);

        // Combine and move
        _velocity.y = _verticalVelocity;
        _velocity += movementDelta + frictionDelta + slopeDelta;
        _motor.Move(_velocity);
        _lastVelocity = _velocity;
    }

    private Vector3 AdjustedMovementAcceleration(Vector3 acc)
    {
        acc *= _settings.movementAcceleration;
        acc.x *= _settings.horizontalAccelerationWeight;
        acc.z *= _settings.verticalAccelerationWeight;
        return acc;
    }

    private void HandleCameraAndBodyRotation()
    {
        var mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        _cameraRotX = Mathf.Clamp(_cameraRotX - mouse.y, _settings.minCameraTilt, _settings.maxCameraTilt);
        transform.Rotate(0, mouse.x, 0);
        _camera.localRotation = Quaternion.Euler(_cameraRotX, _camera.localEulerAngles.y, _camera.localEulerAngles.z);
    }

    private (Vector3 direction, float angle) SlopeDirection()
    {
        var downSlope = Vector3.Cross(_downHit.normal, Vector3.up);
        downSlope = Vector3.Cross(_downHit.normal, downSlope);
        return (downSlope, Vector3.Angle(downSlope, Vector3.up));
    }
}
