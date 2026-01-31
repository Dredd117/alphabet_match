using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars : MonoBehaviour
{
    // Position / motion state
    Vector3 StartPosition;
    Quaternion StartRotation;
    bool AnimateFlag;

    float x_position;
    float y_position;
    float x_velocity;    // units per second
    float y_velocity;    // units per second

    // Gravity in "units per second squared" (time-based, not per frame)
    [SerializeField] float gravity = -450f;

    // Rotation state (degrees per second)
    float rotationSpeed;

    // How far past the bottom of the screen the star must go
    // before we consider it "gone" (in screen pixels).
    [SerializeField] float screenBottomPadding = 50f;

    void Start()
    {
        AnimateFlag = false;

        // Cache where this star lives in the layout
        StartPosition = transform.localPosition;
        StartRotation = transform.localRotation;

        gameObject.SetActive(false);
    }

    public void Reset()
    {
        AnimateFlag = false;

        // Reset motion state to starting position
        x_position = StartPosition.x;
        y_position = StartPosition.y;
        x_velocity = 0f;
        y_velocity = 0f;

        // Reset transform back to its original layout pose
        transform.localPosition = StartPosition;
        transform.localRotation = StartRotation;

        gameObject.SetActive(false);
    }

    public void Animate()
    {
        // Start from the original grid position
        x_position = StartPosition.x;
        y_position = StartPosition.y;

        // Initial launch velocities (units per second).
        // Tweak these for how "strong" the burst is.
        x_velocity = UnityEngine.Random.Range(150f, 350f);
        y_velocity = UnityEngine.Random.Range(250f, 450f);

        // Stronger gravity now that it's per-second²:
        gravity = -1500f;  // tweak this value to taste

        // Random left / right
        if (UnityEngine.Random.value > 0.5f)
            x_velocity *= -1f;

        // --- Rotation flair ---
        // Random spin speed and direction, in degrees/second
        float baseSpin = UnityEngine.Random.Range(180f, 540f);  // 0.5–1.5 full spins per second
        float direction = UnityEngine.Random.value > 0.5f ? 1f : -1f;
        rotationSpeed = baseSpin * direction;

        // Ensure we start from the "correct" resting rotation
        transform.localRotation = StartRotation;

        AnimateFlag = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!AnimateFlag)
            return;

        // Time step for this frame
        float dt = Time.deltaTime;

        // Integrate velocity → position (time-based)
        x_position += dt * x_velocity;
        y_position += dt * y_velocity;

        // Integrate acceleration (gravity) → velocity (time-based)
        y_velocity += gravity * dt;

        // Apply position in local UI space
        transform.localPosition = new Vector3(
            x_position,
            y_position,
            StartPosition.z
        );

        // Apply the spin (around Z axis for UI)
        transform.Rotate(0f, 0f, rotationSpeed * dt, Space.Self);

        // Convert to screen space and check against actual bottom of screen
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, transform.position);

        if (screenPos.y < -screenBottomPadding)
        {
            Reset();
        }
    }
}
