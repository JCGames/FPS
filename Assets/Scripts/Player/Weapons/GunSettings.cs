using UnityEngine;

[CreateAssetMenu(fileName = "GunSettings", menuName = "Player/Weapons/Gun Settings")]
public class GunSettings : ScriptableObject
{
    [Header("Default Settings")]
    public bool isSemiAutomatic;
    public float fireRate = 1f;
    public int bulletDamage = 10;
    public GameObject bulletImpactParticleSystem;
    public GameObject bulletImpactDecal;
    
    [Header("Raycast Settings")]
    public bool isRaycasted;
    public float raycastDistance = 10000f;
}
