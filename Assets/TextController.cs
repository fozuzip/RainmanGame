using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextController : MonoBehaviour {

	public TextMesh textMesh;
	public GameObject particle;
	public BoxCollider2D box;
	Rigidbody2D rb;
	public float fallSpeed;
	public float rotationSpeed;
	public float timeAlive = 6;
	public float particleDuration = 5;
	public Color[] colors;

	public delegate void OnTextActivation(string word);
	public event OnTextActivation textActivation;

	float deathTime;

	void Awake(){
		textMesh = GetComponent<TextMesh> ();
		box = GetComponent<BoxCollider2D> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start(){
		deathTime = timeAlive + Time.time;
		rb.AddTorque (Random.Range(-10f,10f), ForceMode2D.Force);
		if (textMesh.text == "The End.")
			textMesh.color = Color.red;
	}

	// Update is called once per frame
	void Update () {
		//transform.Rotate (0, 0, rotationSpeed * Time.deltaTime);
		//transform.Translate (new Vector2 (0, -fallSpeed * Time.deltaTime), Space.World);
	
		if (Time.time >= deathTime) {
			DieQuiettly();
		}
	}

	void OnCollisionEnter2D (Collision2D collision){
		if (collision.gameObject.tag == "Text") {
			return;
		}
		if (textActivation != null && textMesh != null)
			textActivation (textMesh.text);
		else
			print ("no refference");
		Destroy(textMesh);
		Destroy (box);
		GameObject part = Instantiate (particle , transform.position , Quaternion.identity);
		part.GetComponent<ParticleSystem> ().Play ();
		Destroy(part , particleDuration);

	}

	public void Die(){
		GameObject part = Instantiate (particle , transform.position , Quaternion.identity);
		part.GetComponent<ParticleSystem> ().Play ();
		Destroy(part , particleDuration);
		Object.Destroy (this.gameObject);
	}

	public void DieQuiettly(){
		Object.Destroy (this.gameObject);
	}

}
