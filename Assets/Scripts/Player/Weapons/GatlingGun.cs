using UnityEngine;

public class GatlingGun : Gun
{
    [SerializeField] private Transform _barrel;
    [SerializeField] private float _slowDownSpeed = 1f;
    [SerializeField] private float _speedUpSpeed = 2f;
    [SerializeField] private float _speed = 10f;
    
    [SerializeField] private float _rotationalVelocity;
    
    protected override void OnUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            _rotationalVelocity = Mathf.Lerp(_rotationalVelocity, _speed, Time.deltaTime * _speedUpSpeed);
        }
        else
        {
            _rotationalVelocity = Mathf.Lerp(_rotationalVelocity, 0f, Time.deltaTime * _slowDownSpeed);
        }

        _barrel.Rotate(Vector3.right, _rotationalVelocity * Time.deltaTime);
    }
}
