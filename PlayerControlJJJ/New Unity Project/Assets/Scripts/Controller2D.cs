using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour {

	BoxCollider2D collider;
	//Holds the corners of the player
	RaycastPoints corner;
	//How wide the 'skin' is for raycasting
	public LayerMask collisionMask;
	const float skinWidth = 0.015f;
	//The number of rays being cast in each direction
	public int raysHotizontal = 4;
	public int raysVerticle = 4;
	//Stores directions of collissions! This is very useful
	public CollisionInfo collisions;
	//Used to hold the spacing of the rays being cast in each direction
	float raySpacingVerticle;
	float raySpacingHorizontal;
	
	void Start () 
	{
		collider = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();
	}
	
	/// <summary>
	/// Move the specified velocity.
	/// </summary>
	/// <param name="velocity">Velocity.</param>
	public void Move(Vector3 velocity)
	{
		//update cast points when you go to move
		UpdateRaycastPoints ();
		collisions.Reset ();
		//check if you're going to hit anything vertically and sets verticle velocity appropriately
		if (velocity.y!= 0)
			VerticalCollisions (ref velocity);
		if (velocity.x != 0)
			HorizontalCollisions (ref velocity);

		transform.Translate (velocity);
	}
	/// <summary>
	/// Actvates on a horizontal collision, prevents phasing.
	/// *draws veriticle rays for debug
	/// <param name="velocity">pass by reference the velocity of the player's current movement, edits it to move to edge if an edge exists</param>
	/// </summary>
	void HorizontalCollisions(ref Vector3 velocity)
	{
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;
		for (int i = 0; i < raysHotizontal; i++) 
		{
			Vector2 rayOrigin = (directionX == -1)?corner.bottomLeft:corner.bottomRight;
			rayOrigin += Vector2.up * (raySpacingHorizontal*i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
			Debug.DrawRay(rayOrigin, Vector2.right * directionX *rayLength, Color.red);
			if (hit)
			{
				velocity.x = (hit.distance - skinWidth) * directionX;
				rayLength = hit.distance;
				collisions.left = directionX == -1;
				collisions.right = directionX == 1;
			}
		}
	}
	/// <summary>
	/// Actvates on a Vertical collision, prevents phasing.
	/// *draws veriticle rays for debug
	/// <param name="velocity">pass by reference the velocity of the player's current movement, edits it to move to edge if an edge exists</param>
	/// </summary>
	void VerticalCollisions(ref Vector3 velocity)
	{
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;
		for (int i = 0; i < raysVerticle; i++) 
		{
			Vector2 rayOrigin = (directionY == -1)?corner.bottomLeft:corner.topLeft;
			rayOrigin += Vector2.right * (raySpacingVerticle*i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
			Debug.DrawRay(rayOrigin, Vector2.up * directionY *rayLength, Color.red);
			if (hit)
			{
				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;
				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
	}

	/// <summary>
	/// Holds the corners of the player for raycasting
	/// </summary>
	struct RaycastPoints
	{
		public Vector2 topLeft, topRight, bottomLeft, bottomRight;
	}

	///<summary>
	/// Stores info detailing what side is colliding
	/// </summary>
	public struct CollisionInfo
	{
		public bool above,below,left,right;
		public void Reset() 
		{
			above = below = left = right = false;
		}
	}
	/// <summary>
	/// Updates the raycast points of origin on the skin.
	/// use when the player moves
	/// </summary>
	void UpdateRaycastPoints()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);
		corner.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		corner.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		corner.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		corner.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}
	/// <summary>
	/// Calculates the spacing of the rays.
	/// </summary>
	void CalculateRaySpacing()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);
		raysHotizontal = Mathf.Clamp (raysHotizontal, 2, int.MaxValue);
		raysVerticle = Mathf.Clamp (raysVerticle, 2, int.MaxValue);
		raySpacingHorizontal = bounds.size.y / (raysHotizontal - 1);
		raySpacingVerticle = bounds.size.x / (raysVerticle - 1);

	}
}
