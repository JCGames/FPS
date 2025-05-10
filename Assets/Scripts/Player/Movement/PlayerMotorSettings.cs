using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMotorSettings", menuName = "Player/Movement/Player Motor Settings")]
public class PlayerMotorSettings : ScriptableObject
{
	public bool LockCursorOnAwake = false;
	public bool LockCursorOnFocus = false;
	public bool CanJump = true;

	[Space(10)]
	public float MovementSpeed = 10F;
	[Range(0, 1)] public float VerticalMovementWeight = 1F;
	[Range(0, 1)] public float HorizontalMovementWeight = 1F;

	[Space(10)]
	public KeyCode JumpKey = KeyCode.Space;
	public float JumpForce = 0.02F;
	public float Gravity = 0.05F;
	public float airFriction = 1F;
	public float groundFriction = 10F;

	[Space(10)]
	public float MinCameraTilt = -90F;
	public float MaxCameraTilt = 90F;
}
