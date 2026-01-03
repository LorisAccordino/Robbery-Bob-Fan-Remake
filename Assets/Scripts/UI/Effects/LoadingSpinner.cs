using UnityEngine;

public class LoadingSpinner : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Rotation speed in deg/sec")]
    public float angularSpeed = 180f;

    [Tooltip("If true rotate clockwise, else counterclockwise")]
    public bool clockwise = true;

    void Update()
    {
        float direction = clockwise ? -1f : 1f;
        float rotationAmount = direction * angularSpeed * Time.deltaTime;

        transform.Rotate(0f, 0f, rotationAmount);
    }
}