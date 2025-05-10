using System;
using UnityEngine;

public class Gun : Weapon
{
    [SerializeField] protected GunSettings _settings;
    [SerializeField] protected Transform _cameraTransform;
    [SerializeField] protected Transform _muzzle;
    [SerializeField] protected ParticleSystem _muzzleFlashParticleSystem;
    [SerializeField] private bool _debug;
    
    private float _lastFireTimeStamp;
    protected int _currentAmmo;

    private void Awake()
    {
        _currentAmmo = _settings.totalAmmo;
    }

    private void Start()
    {
        GlobalEvents.CallOnAmmoChanged(_settings.totalAmmo, _currentAmmo);
    }

    private void Update()
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            _currentAmmo = _settings.totalAmmo;
            GlobalEvents.CallOnAmmoChanged(_settings.totalAmmo, _currentAmmo);
        }
        
        OnUpdate();
    }

    private void OnGUI()
    {
        if (_debug)
        {
            GUILayout.Label($"Ammo: {_currentAmmo}/{_settings.totalAmmo}");
        }
    }

    private void Fire()
    {
        // no more ammo
        if (_currentAmmo <= 0) return;
        
        if (_settings.isRaycasted)
        {
            Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, _settings.raycastDistance, ~LayerMask.GetMask("Player"));

            if (hit.collider is not null)
            {
                if (hit.collider.TryGetComponent<Health>(out var health))
                {
                    health.Damage(_settings.bulletDamage);
                }
                
                if (_settings.bulletTrail is not null)
                {
                    Instantiate(_settings.bulletTrail, _muzzle.position, Quaternion.identity).destination = hit.point;
                }
                
                if (_settings.bulletImpactDecal is not null)
                {
                    Instantiate(_settings.bulletImpactDecal, hit.point, Quaternion.LookRotation(hit.normal)).transform.SetParent(hit.transform);
                }
                
                if (_settings.bulletImpactParticleSystem is not null)
                {
                    Instantiate(_settings.bulletImpactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal)).transform.SetParent(hit.transform);
                }
            }
            else
            {
                if (_settings.bulletTrail is not null)
                {
                    Instantiate(_settings.bulletTrail, _muzzle.position, Quaternion.identity).destination = _muzzle.position + _muzzle.forward * 10f;
                }
            }

            _muzzleFlashParticleSystem?.Play();
        }
        
        _currentAmmo--;
        GlobalEvents.CallOnAmmoChanged(_settings.totalAmmo, _currentAmmo);
    }
    
    protected virtual void OnUpdate() 
    { }
}