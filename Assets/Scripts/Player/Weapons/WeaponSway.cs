// Credit: https://gist.github.com/pppoe252110/068db8a845f817570d9dde87c173d930

using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [SerializeField] private Transform weaponTransform;

    [Header("Sway Properties")]
    [SerializeField] private float swaySmooth = 8f;
    [SerializeField] private float swayDamp = 2f;
    [SerializeField] private float posToRotAmount = 1f;
    [SerializeField] private float rotToPosAmount = .1f;
    [SerializeField] private AnimationCurve swayOverDistance = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Rotation Sway")]
    [SerializeField] private float rotationSwayMultiplier = -.2f;
    [SerializeField] private float maxRotSwayAmount = 5f;

    [Header("Position Sway")]
    [SerializeField] private float movementSwayMultiplier = -0.05f;
    [SerializeField] private float maxPosSwayAmount = 0.01f;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private Vector2 _rotSway;
    private Vector3 _posSway;
    private Quaternion _lastRotation;
    private Vector3 _lastPosition;

    private void Start()
    {
        if (!weaponTransform)
            weaponTransform = transform.GetChild(0);

        CacheInitialTransforms();
    }

    private void CacheInitialTransforms()
    {
        _lastRotation = transform.localRotation;
        _lastPosition = transform.position;
        _initialPosition = weaponTransform.localPosition;
        _initialRotation = weaponTransform.localRotation;
    }

    private void LateUpdate()
    {
        CalculateSway();
        ApplySway();
    }

    private void CalculateSway()
    {
        CalculateRotationalSway();
        CalculatePositionalSway();
        ClampSwayValues();
    }

    private void CalculateRotationalSway()
    {
        Quaternion angularVelocity = Quaternion.Inverse(_lastRotation) * transform.rotation;
        _lastRotation = transform.rotation;

        float mouseX = FixAngle(angularVelocity.eulerAngles.y) * rotationSwayMultiplier;
        float mouseY = -FixAngle(angularVelocity.eulerAngles.x) * rotationSwayMultiplier;

        Vector2 result = new Vector2(mouseX, mouseY);
        result *= swayOverDistance.Evaluate(1f - _rotSway.magnitude / maxRotSwayAmount);

        _rotSway += result;
    }

    private void CalculatePositionalSway()
    {
        Vector3 positionDelta = transform.position - _lastPosition;
        _lastPosition = transform.position;

        positionDelta = weaponTransform.InverseTransformDirection(positionDelta) * movementSwayMultiplier;
        positionDelta *= swayOverDistance.Evaluate(1f - _posSway.magnitude / maxPosSwayAmount);
        _posSway += positionDelta;
    }

    private void ClampSwayValues()
    {
        _rotSway = Vector2.ClampMagnitude(_rotSway, maxRotSwayAmount);
        _posSway = Vector3.ClampMagnitude(_posSway, maxPosSwayAmount);
    }

    private void ApplySway()
    {
        float sway = swaySmooth * Time.deltaTime;

        ApplyRotationSway(sway);
        ApplyPositionSway(sway);

        ReduceSwayOverTime(sway * swayDamp, sway * swayDamp);
    }

    private void ApplyRotationSway(float deltaRot)
    {
        Quaternion targetRotation = _initialRotation * Quaternion.Euler(new Vector3(-_rotSway.y - _posSway.y * posToRotAmount * Mathf.Rad2Deg, _rotSway.x + _posSway.x * posToRotAmount * Mathf.Rad2Deg, 0));
        weaponTransform.localRotation = Quaternion.Lerp(weaponTransform.localRotation, targetRotation, deltaRot * swaySmooth);
    }

    private void ApplyPositionSway(float deltaPos)
    {
        Vector3 targetPosition = _initialPosition + _posSway + new Vector3(rotToPosAmount * _rotSway.x * Mathf.Deg2Rad, rotToPosAmount * _rotSway.y * Mathf.Deg2Rad);
        weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, targetPosition, deltaPos * swaySmooth);
    }

    private void ReduceSwayOverTime(float deltaRot, float deltaPos)
    {
        _rotSway = Vector2.Lerp(_rotSway, Vector2.zero, deltaRot);
        _posSway = Vector3.Lerp(_posSway, Vector3.zero, deltaPos);
    }

    private float FixAngle(float angle)
    {
        // Normalize the angle to be within -180 to 180 degrees
        return Mathf.Repeat(angle + 180f, 360f) - 180f;
    }
}