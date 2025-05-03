using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
	[SerializeField] private PlayerMotorSettings _settings;
	[SerializeField] private Transform _camera;
	
	private CharacterController _motor;

	private float _cameraRotX = 0;
	private float _angularVelocity = 0;

	private void Awake()
	{
		_motor = GetComponent<CharacterController>();

		if (_settings.LockCursorOnAwake)
			Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		HandleGravityAndJump();
		HandlePosition();
		HandleCameraAndBodyRotation();

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (_settings.LockCursorOnFocus)
			Cursor.lockState = CursorLockMode.Locked;
	}
	
	private void HandleGravityAndJump()
	{
		if (!_settings.CanJump) return;
		
		if (_motor.isGrounded)
		{
			_angularVelocity = -0.01f;

			if (Input.GetKeyDown(_settings.JumpKey))
			{
				_angularVelocity = _settings.JumpForce;
			}
		}
		else
		{
			_angularVelocity -= _settings.Gravity * Time.deltaTime;
		}
	}

	private void HandlePosition()
	{
		var move = new Vector3(Input.GetAxis("Horizontal"), _angularVelocity, Input.GetAxis("Vertical"));

		move.x *= _settings.HorizontalMovementWeight * _settings.MovementSpeed * Time.deltaTime;
		move.z *= _settings.VerticalMovementWeight * _settings.MovementSpeed * Time.deltaTime;

		// NOTE: a better design needs to be implemented here.
		//if (move.x != 0 && move.z != 0)
		//{
		//	move.x *= 0.5f;
		//	move.z *= 0.5f;
		//}

		_motor.Move(transform.TransformDirection(move));
	}

	private void HandleCameraAndBodyRotation()
	{
		var input = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

		_cameraRotX = Mathf.Clamp(_cameraRotX - input.x, _settings.MinCameraTilt, _settings.MaxCameraTilt);
		transform.Rotate(0, input.y, 0);

		_camera.localRotation = Quaternion.Euler(_cameraRotX, _camera.localEulerAngles.y, _camera.localEulerAngles.z);
	}
}
