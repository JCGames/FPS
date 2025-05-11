using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerMotorSettings", menuName = "Player/Movement/Player Motor Settings")]
public class PlayerMotorSettings : ScriptableObject
{
	public bool lockCursorOnAwake = false;
	public bool lockCursorOnFocus = false;
	public bool canJump = true;

	[Space(10)]
	public float movementAcceleration = 10F;
	[Range(0, 1)] public float verticalAccelerationWeight = 1F;
	[Range(0, 1)] public float horizontalAccelerationWeight = 1F;
	[Range(0, 1)] public float frictionOnGround = 0.3F;
	[Range(0, 1)] public float frictionInAir = 0.001F;
 	
	[Space(10)]
	public KeyCode jumpKey = KeyCode.Space;
	public float jumpForce = 0.02F;
	public float gravity = 0.05F;

	[Space(10)] 
	public float slopeAcceleration = 5f;
	public float groundRaycastDistance = 10F;
	public float groundRaycastRadius = 2f;

	[Space(10)]
	public float minCameraTilt = -90F;
	public float maxCameraTilt = 90F;
}
