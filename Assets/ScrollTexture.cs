using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour {

	public float speed = 0;

	public Material material;

	// Update is called once per frame
	void Update () {
		material.mainTextureOffset = new Vector2 (0f, Time.time * speed);
	}
}
