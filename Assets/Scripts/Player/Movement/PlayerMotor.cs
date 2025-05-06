using System;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
	[SerializeField] private PlayerMotorSettings _settings;
	[SerializeField] private Transform _camera;
	[SerializeField] private bool _debug;
	
	private CharacterController _motor;

	private float _cameraRotX;
	private Vector3 _velocity;
	private Vector3 _velocitySmoothed;
	private float _mouseX;
	private float _mouseY;
	private bool _jumpKeyPressed;
	private bool _playerGrounded;
	
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
		_playerGrounded = _motor.isGrounded;
		_mouseY = Input.GetAxis("Mouse Y");
		_mouseX = Input.GetAxis("Mouse X");
		
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
		
		if (!_playerGrounded)
		{
			move = transform.TransformDirection(move);
			
			_velocitySmoothed.x = Mathf.Lerp(_velocitySmoothed.x, _velocity.x, Time.deltaTime * 500f) + move.x * _settings.MovementSpeed * 0.25f;
			_velocitySmoothed.y = _velocity.y;
			_velocitySmoothed.z = Mathf.Lerp(_velocitySmoothed.z, _velocity.z, Time.deltaTime * 500f) + move.z * _settings.MovementSpeed * 0.25f;
			
			_motor.Move(_velocitySmoothed * Time.deltaTime);
			return;
		}
		
		// this is our velocity vector
		var velocity = new Vector3
		{
			x = move.x * _settings.HorizontalMovementWeight * _settings.MovementSpeed,
			y = _velocity.y,
			z = move.z * _settings.VerticalMovementWeight * _settings.MovementSpeed
		};
		
		// transform this final move vector into world space making
		// its forward direction our forward direction 
		velocity = transform.TransformDirection(velocity);
		_velocity = velocity;
		
		_velocitySmoothed.x = Mathf.Lerp(_velocitySmoothed.x, _velocity.x, Time.deltaTime * 500f);
		_velocitySmoothed.y = _velocity.y;
		_velocitySmoothed.z = Mathf.Lerp(_velocitySmoothed.z, _velocity.z, Time.deltaTime * 500f);
		
		_motor.Move(_velocitySmoothed * Time.deltaTime);
	}

	private void HandleCameraAndBodyRotation()
	{
		var input = new Vector2(_mouseY, _mouseX);
		
		// clamp the extents of the cameras up and down look axis
		_cameraRotX = Mathf.Clamp(_cameraRotX - input.x, _settings.MinCameraTilt, _settings.MaxCameraTilt);
		
		// rotate the player around the y axis
		transform.Rotate(0, input.y, 0);

		// rotate the camera around the x axis
		_camera.localRotation = Quaternion.Euler(_cameraRotX, _camera.localEulerAngles.y, _camera.localEulerAngles.z);
	}
}
