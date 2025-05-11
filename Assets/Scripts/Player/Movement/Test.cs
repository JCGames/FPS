using UnityEngine;

public class Test : MonoBehaviour
{
    public CharacterController motor;

    private void Start()
    {
        motor.Move(Vector3.left * 100);
    }
}
