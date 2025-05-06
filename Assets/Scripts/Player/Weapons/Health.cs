using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public UnityEvent<int> OnKilled;
    public UnityEvent<int> OnHealed;
    public UnityEvent OnFullyHealed;
    
    [SerializeField] private int _hitPoints = 100;

    private int _fullHitPoints;
    private bool _isDead;

    private void Awake()
    {
        _fullHitPoints = _hitPoints;
    }
    
    public void Damage(int amount)
    {
        if (_isDead) return;
        
        _hitPoints -= amount;
        
        if (_hitPoints <= 0)
        {
            _hitPoints = 0;
            _isDead = true;
            
            OnKilled?.Invoke(_hitPoints);
        }
    }

    public void Heal()
    {
        _hitPoints = _fullHitPoints;
        _isDead = false;
        
        OnFullyHealed?.Invoke();
    }
    
    public void Heal(int amount)
    {
        _hitPoints += amount;
        _isDead = false;

        if (_hitPoints > _fullHitPoints)
        {
            _hitPoints = _fullHitPoints;
        }
        
        OnHealed?.Invoke(_hitPoints);
    }
}
