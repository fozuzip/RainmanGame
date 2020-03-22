using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float horizontalSpeed;
	public float verticalSpeed;
	public float smoothingTime;

	SpriteRenderer spriteRenderer;
	CollisionController collisions;

	public Sprite falling;
	public Sprite grabbing;
	public Sprite flying;

	public float animChangeTime = .2f;
	float canChangeAnimTime;

	float velocityXsmoothing;
	float velocityYsmoothing;

	Vector2 directionalInput;
	Vector2 velocity;
	// Use this for initialization
	void Start () {
		collisions = GetComponent<CollisionController> ();
		directionalInput = new Vector2();
		velocity = new Vector2 ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		directionalInput.x = Input.GetAxisRaw ("Horizontal");
		directionalInput.y = Input.GetAxisRaw ("Vertical");

		float targetVelocityX = directionalInput.x * horizontalSpeed;
		float targetVelocityY =  directionalInput.y * verticalSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXsmoothing, smoothingTime);
		velocity.y = Mathf.SmoothDamp (velocity.y, targetVelocityY, ref velocityYsmoothing, smoothingTime);

		if (Time.time >= canChangeAnimTime) {
			if (velocity.y >= 0f ) {
				spriteRenderer.sprite = flying;
			} else {
				spriteRenderer.sprite = falling;
			}
		}
		if (velocity.x < 0) {
			spriteRenderer.flipX = true;
		} else if(velocity.x > 0){
			spriteRenderer.flipX = false;
		}



		Vector2 moveAmount = velocity * Time.deltaTime;


		if(Time.time >= canChangeAnimTime && collisions.CollisionCheckBool (moveAmount * 20)){
			spriteRenderer.sprite = grabbing;
			canChangeAnimTime = Time.time + 2*animChangeTime;
		}

		transform.Translate (moveAmount);
	}
}
