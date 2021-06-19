using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerMovement : MonoBehaviour
{
	Controller2D controller;

	public float moveSpeed = 6;
	public float jumpForce = 10;
	public float gravityScale = -2;
	public float airStrafeSpeed = 5;

	Vector3 velocity;

	void Start()
	{
		controller = GetComponent<Controller2D>();
	}

	void Update()
	{
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		// Player's horizontal speed based on grounding.
		if(!controller.colInfo.bottom)
        {
			velocity.x = input.x * airStrafeSpeed;
		}
		else
        {
			velocity.x = input.x * moveSpeed;
		}

		velocity.y += gravityScale * Time.deltaTime;

		if(controller.colInfo.left || controller.colInfo.right)
        {
			velocity.x = 0;
        }

		if (controller.colInfo.bottom)
		{
			velocity.y = input.y * jumpForce;
		}

		controller.Move(velocity * Time.deltaTime);
	}
}
