﻿using Mirror;
using UnityEngine;

public class PlayerMovementController : NetworkBehaviour
{
	[SerializeField] private float walkSpeed = 2;
	[SerializeField] private float runSpeed = 6;
	[SerializeField] private float gravity = -12;
	[SerializeField] private float jumpHeight = 1;
	[SerializeField] private float mouseSpeed = 100f;

	[Range(0, 1)]
	public float airControlPercent;

	//[SerializeField] private float turnSmoothTime = 0.2f;
	//float turnSmoothVelocity;

	[SerializeField] private float speedSmoothTime = 0.1f;
	float speedSmoothVelocity;
	float currentSpeed;
	float velocityY;
	bool isRunning = false;

	//camera
	float xRotation = 0f;
	Camera playerCamera = null;
	Vector3 offSet = new Vector3(0f, .7f, 0f);
	//[SerializeField] Transform weaponHolster = null;

	//Animator animator;
	CharacterController controller;

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
		controller = GetComponent<CharacterController>();
		playerCamera = Camera.main;
		playerCamera.transform.SetParent(transform);
		playerCamera.transform.localPosition = offSet;
		//animator = GetComponent<Animator>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = !Cursor.visible;
	}

	//private void Start()
	//{
	//	//if (!isLocalPlayer) { return; }
	//
	//	if (isLocalPlayer)
	//	{
	//	}
	//	else
	//	{
	//		playerCamera.enabled = false;
	//		GetComponentInChildren<AudioListener>().enabled = false;
	//	}
	//}

	void Update()
	{
		if (!isLocalPlayer)
			return;

		// input
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		Vector2 inputDir = input.normalized;
		float mouseX = Input.GetAxisRaw("Mouse X") * mouseSpeed * Time.deltaTime;
		float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSpeed * Time.deltaTime;

		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);

		playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

		if (Input.GetKeyDown(KeyCode.LeftShift))
			isRunning = !isRunning;

		transform.Rotate(Vector3.up * mouseX);
		Move(inputDir, isRunning);

		if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
		{
			Jump();
		}

		// animator
		//float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
		//animator.SetFloat("SpeedPercentage", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
	}

	void Move(Vector2 inputDir, bool running)
	{
		//if (inputDir != Vector2.zero)
		//{
		//	float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
		//	transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
		//}

		float targetSpeed = (running ? runSpeed : walkSpeed) * inputDir.magnitude;
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

		velocityY += Time.deltaTime * gravity;
		Vector3 velocity = (transform.right * inputDir.x + transform.forward * inputDir.y) * currentSpeed + Vector3.up * velocityY;

		controller.Move(velocity * Time.deltaTime);
		currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

		if (controller.isGrounded)
		{
			velocityY = 0;
		}
	}

	void Jump()
	{
		float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
		velocityY = jumpVelocity;
	}

	float GetModifiedSmoothTime(float smoothTime)
	{
		if (controller.isGrounded)
		{
			return smoothTime;
		}

		if (airControlPercent == 0)
		{
			return float.MaxValue;
		}
		return smoothTime / airControlPercent;
	}
}