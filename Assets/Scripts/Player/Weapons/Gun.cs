using System;
using UnityEngine;

public class Gun : Weapon
{
    [SerializeField] private GunSettings _settings;

    private float _lastFireTimeStamp;
    
    public void Update()
    {
        if (_settings.isSemiAutomatic)
        {
            if (Input.GetMouseButtonDown(0) && _lastFireTimeStamp < Time.time)
            {
                _lastFireTimeStamp = Time.time + _settings.fireRate;
                Fire();
            }
        }
        else
        {
            if (Input.GetMouseButton(0) && _lastFireTimeStamp < Time.time)
            {
                _lastFireTimeStamp = Time.time + _settings.fireRate;
                Fire();
            }
        }
    }

    private void Fire()
    {
        
    }
}