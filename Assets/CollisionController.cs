using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class CollisionController : MonoBehaviour {
	public LayerMask collisionMask;
	public bool debug;
	public const float skinWidth = .015f;
	public float dstBetweenRays = .25f;

	public CollisionInfo collisions;
	[HideInInspector]
	public int horizontalRayCount;
	[HideInInspector]
	public int verticalRayCount;

	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	[HideInInspector]
	public BoxCollider2D collider;
	public RaycastOrigins raycastOrigins;

	public virtual void Awake() {
		collider = GetComponent<BoxCollider2D> ();
	}

	public virtual void Start() {
		CalculateRaySpacing ();
		collisions = new CollisionInfo ();	
	}

	public void UpdateRaycastOrigins() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	public void CalculateRaySpacing() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		float boundsWidth = bounds.size.x;
		float boundsHeight = bounds.size.y;

		horizontalRayCount = Mathf.RoundToInt (boundsHeight / dstBetweenRays);
		verticalRayCount = Mathf.RoundToInt (boundsWidth / dstBetweenRays);
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	public Vector2 CollisionCheck(Vector2 moveAmount){
		UpdateRaycastOrigins ();
		collisions.Reset ();
		if (Mathf.Abs (moveAmount.x) > 0) { 
			moveAmount = HorizontalCollisions (moveAmount);
		}
		if (Mathf.Abs (moveAmount.y) > 0) { 
			moveAmount = VerticalCollisions(moveAmount);
		}
		return moveAmount;
	}

	public bool CollisionCheckBool(Vector2 moveAmount){
		Vector2 newAmount = CollisionCheck (moveAmount);
		if (newAmount != moveAmount) {
			return true;
		}
		return false;
	}

	public Vector2 VerticalCollisions (Vector2 moveAmount){
		float direction = Mathf.Sign (moveAmount.y);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;

		if (Mathf.Abs(moveAmount.y) < skinWidth) {
			rayLength = 2*skinWidth;
		}

		for (int i = 0; i < verticalRayCount; i ++) {
			Vector2 rayOrigin = (direction == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, moveAmount, rayLength, collisionMask);

			if(debug) Debug.DrawRay(rayOrigin, moveAmount*rayLength,Color.red);

			if (hit) {

				moveAmount.y = (hit.distance - skinWidth) * direction;
				rayLength = hit.distance;

				collisions.left = direction == -1;
				collisions.right = direction == 1;
			}
		}

		return moveAmount;
	}

	public Vector2 HorizontalCollisions(Vector2 moveAmount) {
		float direction = Mathf.Sign (moveAmount.x);
		float rayLength = Mathf.Abs (moveAmount.x) + skinWidth;

		if (Mathf.Abs(moveAmount.x) < skinWidth) {
			rayLength = 2*skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (direction == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, moveAmount, rayLength, collisionMask);

			if(debug) Debug.DrawRay(rayOrigin, moveAmount.normalized *rayLength,Color.red);

			if (hit) {

				moveAmount.x = (hit.distance - skinWidth) * direction;
				rayLength = hit.distance;

				collisions.left = direction == -1;
				collisions.right = direction == 1;
			}
		}
		return moveAmount;
	}

	public struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public void Reset() {
			above = below = false;
			left = right = false;
		}
	}


}