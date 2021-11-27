using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[Header("Input Settings")]
    [SerializeField] InputAction movement;
    [SerializeField] InputAction fire;

	[Header("General Setup Settings")]
    [Tooltip("How fast ship moves up, down, left, right, based on player input")] [SerializeField] float controlSpeed = 40f;
	[Tooltip("How far player can move horizontally")] [SerializeField] float xRange = 25f;
	[Tooltip("How far player can move vertically")] [SerializeField] float yRange = 15f;

	[Header("Laser gun array")]
	[Tooltip("Add player lasers here")][SerializeField] GameObject[] lasers; 

	[Header("Screen position based tuning")]
	[SerializeField] float positionPitchFactor = -2f;
	[SerializeField] float positionYawFactor = 2f;

	[Header("Player input based tuning")]
	[SerializeField] float controlPitchFactor = -15f;
	[SerializeField] float controlRollFactor = -30f;

	float xThrow, yThrow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

	void OnEnable()
	{
        movement.Enable();
		fire.Enable();
	}

	void OnDisable()
	{
        movement.Disable();
		fire.Enable();
	}

	// Update is called once per frame
	void Update()
	{
		ProcessTranslation();
		ProcessRotation();
		ProcessFiring();
	}

	void ProcessRotation()
	{
		float pitchDueToPosition = transform.localPosition.y * positionPitchFactor;
		float pitchDueToControlThrow = yThrow * controlPitchFactor;

		float pitch = pitchDueToPosition + pitchDueToControlThrow;
		float yaw = transform.localPosition.x * positionYawFactor;
		float roll = xThrow * controlRollFactor;

		transform.localRotation = Quaternion.Euler(pitch, yaw, roll);
	}

	void ProcessTranslation()
	{
		xThrow = movement.ReadValue<Vector2>().x;
		yThrow = movement.ReadValue<Vector2>().y;

		float xOffset = xThrow * Time.deltaTime * controlSpeed;
		float rawXPos = transform.localPosition.x + xOffset;
		float clampedXPos = Mathf.Clamp(rawXPos, -xRange, xRange);

		float yOffset = yThrow * Time.deltaTime * controlSpeed;
		float rawYPos = transform.localPosition.y + yOffset;
		float clampedYPos = Mathf.Clamp(rawYPos, -yRange, yRange);

		transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
	}

	void ProcessFiring()
	{
		var isActive = fire.ReadValue<float>() > 0.5;
		SetLasersActive(isActive);
	}

	void SetLasersActive(bool isActive)
	{
		foreach(GameObject laser in lasers)
		{
			var emissionModule = laser.GetComponent<ParticleSystem>().emission;
			emissionModule.enabled = isActive;
		}
	}


}
