using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
	[SerializeField] private PlayerMotorSettings _settings;
	[SerializeField] private Transform _camera;
	[SerializeField] private bool _debug;
	
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
		if (transform.position.y < -10)
		{
			_motor.enabled = false;
			transform.position = Vector3.up * 10f;
			_motor.enabled = true;
		}
		
		_slope = SlopeDirection();
		Physics.SphereCast(transform.position, _settings.groundRaycastRadius, Vector3.down, out _downHit, _settings.groundRaycastDistance, ~LayerMask.GetMask("Player"));
		_playerGrounded = _motor.isGrounded;
		
		// only want to jump if we are grounded
		if (_settings.canJump && _playerGrounded && Input.GetKeyDown(_settings.jumpKey))
		{
			_jumpKeyPressed = true;
		}
		
		HandleGravityAndJump();
		HandlePosition();
		HandleCameraAndBodyRotation();
	}
	
	private void OnApplicationFocus(bool focus)
	{
		if (_settings.lockCursorOnFocus)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
	
	private void OnGUI()
	{
		if (_debug)
		{
			GUILayout.Label("IsGrounded: " + _motor.isGrounded);
			GUILayout.Label("Slope Angle: " + _slope.angle);
			GUILayout.Label("Velocity: " + transform.InverseTransformDirection(_velocity));
		}
	}

	private void OnDrawGizmos()
	{
		if (_debug)
		{
			var original = Gizmos.color;
			Gizmos.color = Color.green;

			Gizmos.DrawRay(transform.position, _velocity);
			
			Gizmos.color = original;
		}
	}

	private void HandleGravityAndJump()
	{
		if (_playerGrounded)
		{
			// add a sticky force to make our player stick to the ground
			// should improve behaviour on downwards slopes
			// this is basically the equal and opposite force when an 
			// object is grounded
			_verticalVelocity = -0.5f;
			
			if (_jumpKeyPressed)
			{
				// apply a jump force to the player
				_verticalVelocity = _settings.jumpForce;
				_jumpKeyPressed = false;
			}
		}
		else
		{
			// apply gravity to the player
			_verticalVelocity -= _settings.gravity * Time.deltaTime;
		}
	}

	private void HandlePosition()
	{
		// this is just our input vector
		var moveAcceleration = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		moveAcceleration = Vector3.ClampMagnitude(moveAcceleration, 1);
		moveAcceleration = transform.TransformDirection(AdjustedMovementAcceleration(moveAcceleration));

		// normal acceleration * time
		var normalVelocityDelta = _playerGrounded ? moveAcceleration * Time.deltaTime : moveAcceleration * (0.1f * Time.deltaTime);
		
		var slopeVelocityDelta = _slope.angle > _settings.minAngleForSlope ? 
			_slope.dir * (_slope.angle * 0.00278f * _settings.slopeAcceleration * Time.deltaTime) : 
			Vector3.zero;

		var frictionalCoefficient = _playerGrounded ? _settings.frictionOnGround : _settings.frictionInAir;
		
		// current acceleration * coefficient * time
		var frictionalVelocityDelta = -_lastVelocity / Time.deltaTime * (frictionalCoefficient * Time.deltaTime);
		
		_velocity.y = _verticalVelocity;

		_velocity += normalVelocityDelta + frictionalVelocityDelta + slopeVelocityDelta;

		_motor.Move(_velocity);

		_lastVelocity = _velocity;
	}

	private Vector3 AdjustedMovementAcceleration(Vector3 movementAcceleration)
	{
		movementAcceleration *= _settings.movementAcceleration;
		movementAcceleration.x *= _settings.horizontalAccelerationWeight;
		movementAcceleration.z *= _settings.verticalAccelerationWeight;
		return movementAcceleration;
	}

	private void HandleCameraAndBodyRotation()
	{
		var mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		
		// clamp the extents of the cameras up and down look axis
		_cameraRotX = Mathf.Clamp(_cameraRotX - mouse.y, _settings.minCameraTilt, _settings.maxCameraTilt);
		
		// rotate the player around the y axis
		transform.Rotate(0, mouse.x, 0);

		// rotate the camera around the x axis
		_camera.localRotation = Quaternion.Euler(_cameraRotX, _camera.localEulerAngles.y, _camera.localEulerAngles.z);
	}

	private (Vector3 direction, float angle) SlopeDirection()
	{
		var downSlope = Vector3.Cross(_downHit.normal, Vector3.up);
		downSlope = Vector3.Cross(_downHit.normal, downSlope);
		return (downSlope, Vector3.Angle(downSlope, Vector3.up));
	}
}
