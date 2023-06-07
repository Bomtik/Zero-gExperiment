using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScientistWalk : MonoBehaviour {

	float dirX, moveSpeed;

	Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		moveSpeed = 1f;
	}
	
	// Update is called once per frame
	void Update () {
		dirX = Input.GetAxisRaw ("Horizontal") * moveSpeed * Time.deltaTime;

		//transform.position = new Vector2 (transform.position.x + dirX, transform.position.y);

		if (dirX != 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("shoot")) {
			anim.SetBool ("isWalking", true);
		}
		else {
			anim.SetBool ("isWalking", false);
		}

		if (Input.GetButtonDown ("Fire1") && !anim.GetCurrentAnimatorStateInfo(0).IsName("shoot")) 
		{
			anim.SetBool ("isWalking", false);
			anim.SetTrigger ("hit");
		}

	}
}
