using UnityEngine;

/// <summary>
/// Smoothly follows a target (player) while keeping a fixed rotation.
/// Adds optional natural sway to the camera's position for a more organic feel.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The transform of the player or object to follow")]
    public Transform target;

    [Header("Follow Settings")]
    [Tooltip("Position offset relative to the target")]
    public Vector3 offset = new Vector3(0f, 5f, 0f);

    [Tooltip("How quickly the camera follows the target")]
    public float smoothTime = 0.3f;

    [Header("Camera Sway")]
    [Tooltip("Amplitude of camera sway for natural motion")]
    public float swayAmplitude = 0.5f;

    [Tooltip("Frequency of camera sway oscillation")]
    public float swayFrequency = 0.5f;

    // Internal velocity used for SmoothDamp
    private Vector3 velocity = Vector3.zero;

    // Fixed rotation for the camera
    private readonly Quaternion fixedRotation = Quaternion.Euler(90f, 0f, 0f);

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: Target not assigned!");
            enabled = false;
            return;
        }

        // Set initial rotation to fixed rotation
        transform.rotation = fixedRotation;
    }

    void LateUpdate()
    {
        // Desired position based on target + offset
        Vector3 targetPosition = target.position + offset;

        // Smoothly move the camera toward the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Add optional sway effect to position only
        float swayX = Mathf.Sin(Time.time * swayFrequency) * swayAmplitude;
        float swayY = Mathf.Cos(Time.time * swayFrequency * 0.5f) * swayAmplitude * 0.5f;

        transform.position += new Vector3(swayX, swayY, 0f);

        // Keep the rotation fixed, independent of target
        transform.rotation = fixedRotation;
    }
}