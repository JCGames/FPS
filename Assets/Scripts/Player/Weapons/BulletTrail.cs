using System;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    [NonSerialized] public Vector3 destination;
    public float speed = 1f;
    private Transform _transform;

    private void Awake()
    {
        // caching the transform
        _transform = transform;
    }

    private void Update()
    {
        if ((destination - _transform.position).sqrMagnitude > 1f)
        {
            _transform.rotation = Quaternion.LookRotation(destination - _transform.position);
            _transform.position = Vector3.MoveTowards(_transform.position, destination, speed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
