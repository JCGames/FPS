using UnityEngine;

[CreateAssetMenu(fileName = "GunSettings", menuName = "Player/Weapons/Gun Settings")]
public class GunSettings : ScriptableObject
{
    public bool isSemiAutomatic;
    public float fireRate = 1f;
    public GameObject muzzleFlash;
    public GameObject bulletImpactParticleSystem;
    public GameObject bulletImpactDecal;
}
