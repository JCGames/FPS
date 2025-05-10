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
	private float _friction;
	
	private RaycastHit _downHit;
	private Vector3 _slopeDirection;
	
	private void Awake()
	{
		_motor = GetComponent<CharacterController>();

		if (_settings.LockCursorOnAwake)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	private void Update()
	{
		Physics.Raycast(transform.position, Vector3.down, out _downHit, 100F, ~LayerMask.GetMask("Player"));
		_playerGrounded = _motor.isGrounded;
		
		// only want to jump if we are grounded
		if (_settings.CanJump && _playerGrounded && Input.GetKeyDown(_settings.JumpKey))
		{
			_jumpKeyPressed = true;
		}
		
		HandleGravityAndJump();
		HandlePosition();
		HandleCameraAndBodyRotation();
	}
	
	private void OnApplicationFocus(bool focus)
	{
		if (_settings.LockCursorOnFocus)
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
			Physics.Raycast(transform.position, Vector3.down, out var hit, Mathf.Infinity, LayerMask.GetMask("Ground"));
			GUILayout.Label("Distance From Ground: " + hit.distance);
			
			GUILayout.Label("Velocity: " + _velocity);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, _slopeDirection);
		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, Vector3.down * 100f);
		Gizmos.DrawWireSphere(transform.position + Vector3.down * 100f, 2f);
	}

	private void HandleGravityAndJump()
	{
		if (_playerGrounded)
		{
			// add a sticky force to make our player stick to the ground
			// should improve behaviour on downwards slopes
			_velocity.y = -0.01f;
			
			if (_jumpKeyPressed)
			{
				// apply a jump force to the player
				_velocity.y = Mathf.Sqrt(_settings.JumpForce * 2.0f * _settings.Gravity);
				_jumpKeyPressed = false;
			}
		}
		else
		{
			// apply gravity to the player
			_velocity.y -= _settings.Gravity * Time.deltaTime;
		}
	}

	private void HandlePosition()
	{
		// this is just our input vector
		var move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		
		// prevents diagonal movements from being faster than regular movements
		move = Vector3.ClampMagnitude(move, 1);
		move = transform.TransformDirection(move);
		
		// this is our velocity vector
		var velocity = new Vector3
		{
			x = move.x * _settings.HorizontalMovementWeight * _settings.MovementSpeed,
			y = _velocity.y,
			z = move.z * _settings.VerticalMovementWeight * _settings.MovementSpeed
		};
		
		if (_playerGrounded)
		{
			_friction = _settings.groundFriction;
			
			var (dir, angle) = SlopeDirection();
			
			if (angle > 110)
			{
				velocity = dir * 10f;
				_slopeDirection = velocity;
				_friction = 50f;
			}
		}
		else
		{
			_friction = _settings.airFriction;
		}

		// var distance = Vector3.Distance(_velocity, velocity);
		//
		// if (distance > 0)
		// {
		// 	_friction /= distance;
		// }
		
		_velocity = Vector3.MoveTowards(_velocity, velocity, _friction * Time.deltaTime);
		
		// transform this final move vector into world space making
		// its forward direction our forward direction
		_motor.Move(_velocity * Time.deltaTime);
	}

	private void HandleCameraAndBodyRotation()
	{
		var mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		
		// clamp the extents of the cameras up and down look axis
		_cameraRotX = Mathf.Clamp(_cameraRotX - mouse.y, _settings.MinCameraTilt, _settings.MaxCameraTilt);
		
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
