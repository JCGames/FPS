using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Gun : Weapon
{
    [SerializeField] private GunSettings _settings;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _muzzle;
    [SerializeField] private ParticleSystem _muzzleFlashParticleSystem;
    
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
        
        OnUpdate();
    }

    protected virtual void OnUpdate() 
    { }

    private void Fire()
    {
        if (_settings.isRaycasted)
        {
            Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, _settings.raycastDistance, ~LayerMask.GetMask("Player"));

            if (hit.collider is not null && hit.collider.TryGetComponent<Health>(out var health))
            {
                health.Damage(_settings.bulletDamage);
            }

            _muzzleFlashParticleSystem?.Play();

            if (_settings.bulletImpactDecal is not null)
            {
                Instantiate(_settings.bulletImpactDecal, hit.point, Quaternion.LookRotation(hit.normal)).transform.SetParent(hit.transform);
            }

            if (_settings.bulletImpactParticleSystem is not null)
            {
                Instantiate(_settings.bulletImpactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal)).transform.SetParent(hit.transform);
            }
        }
    }
}