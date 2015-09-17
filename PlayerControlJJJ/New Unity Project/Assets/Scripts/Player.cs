using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {


	Controller2D controller;
	//Sprites
	public Sprite leftSpriteAir;
	public Sprite rightSpriteAir;
	public Sprite leftSprite;
	public Sprite rightSprite;
	//player facing info
	bool inAir, left;
	//physics values
	float gravity;
	float jumpVelocity;
	//editable settings
	public float jumpHeight = 2.7f;
	public float jumpTime = 0.5f;
	public float moveSpeed = 6;
	//smoothers
	float airAccelerationTime = 0.2f;
	float groundAccelerationTime = 0.1f;
	float velocitySmoothingX;
	//velocity!
	Vector3 velocity;

	void Start () {
		controller = GetComponent<Controller2D> ();
		left = true;
		//calculate the physics values based on the settings
		gravity = -(2 * jumpHeight) / Mathf.Pow (jumpTime, 2);
		jumpVelocity = Mathf.Abs (gravity) * jumpTime;
	}

	void Update () {
		//Sprite Management
		if (controller.collisions.below)//Checks when lands to turn off jump sprite
			inAir = false;
		if (inAir) //If in air
		{
			if (velocity.x < 0){ //If moving left
				GetComponent<SpriteRenderer> ().sprite = leftSpriteAir;
				left = true;
			}
			else if (velocity.x > 0){ //If moving right
				GetComponent<SpriteRenderer> ().sprite = rightSpriteAir;
				left = false;
			}
			else
			{
				if (left)
					GetComponent<SpriteRenderer> ().sprite = leftSpriteAir;
				else
					GetComponent<SpriteRenderer> ().sprite = rightSpriteAir;
			}
			
		} 
		else //If on ground
		{
			if (velocity.x < 0){
				GetComponent<SpriteRenderer> ().sprite = leftSprite;
				left = true;
			}
			else if (velocity.x > 0){
				GetComponent<SpriteRenderer> ().sprite = rightSprite;
				left = false;
			}
			else
			{
				if (left)
					GetComponent<SpriteRenderer> ().sprite = leftSprite;
				else
					GetComponent<SpriteRenderer> ().sprite = rightSprite;
			}
		}		
		//reset gravity if touching the ground
		if (controller.collisions.above || controller.collisions.below) 
		{
			velocity.y = 0;
		}
		//prevents climbing up walls (Temporary? still a bit buggy)
		if (controller.collisions.left || controller.collisions.right) 
		{
			velocity.x = 0;
		}
		//jump
		if (Input.GetKeyDown (KeyCode.Space) && controller.collisions.below)
		{
			velocity.y = jumpVelocity;
			inAir = true; //Used for jump sprite
		}
		//movement
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
		float smoothToVelocity = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp(velocity.x,smoothToVelocity, ref velocitySmoothingX, (controller.collisions.below?groundAccelerationTime:airAccelerationTime));
		velocity.y += gravity * Time.deltaTime;
		controller.Move (velocity * Time.deltaTime);
	}
}
