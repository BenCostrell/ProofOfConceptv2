﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Attack {

	private float speed;


	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if (!GetComponent<Renderer> ().isVisible) {
			Destroy (gameObject);
		}
	}

	public override void Init(GameObject player){
		animTrigger = "ThrowFireball";
		cooldown = 1;
		castDuration = 0.2f;
		baseKnockback = 8;
		knockbackGrowth = 0.5f;
		damage = 1;
		speed = 10;
		isProjectile = true;
		isMelee = false;
		onCastAudio = Resources.Load ("Sounds/Abilities/Fireball") as AudioClip;

		base.Init (player);

		float angle = player.GetComponent<Player>().effectiveRotation;
		Vector3 direction = new Vector3 (-Mathf.Cos(angle*Mathf.Deg2Rad), -Mathf.Sin(angle*Mathf.Deg2Rad));

		GetComponent<Rigidbody2D> ().velocity = speed * direction;
		transform.rotation = Quaternion.Euler (0, 0, angle);
		transform.position += 1.5f * direction;
	}

	protected override Vector3 GetDirectionHit(GameObject playerHit){
		return GetComponent<Rigidbody2D> ().velocity.normalized;
	}

	protected override void HitPlayer(GameObject player){
		base.HitPlayer (player);
		Destroy (gameObject);
	}

	public override void OnCastFinish ()
	{
	}
}