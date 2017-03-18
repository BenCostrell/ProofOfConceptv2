﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour {

	public enum Type {None, BasicAttack, Fireball, Lunge, Sing, Shield, Wallop, Pull};

	public float cooldown;
	public GameObject parentPlayer;
	public float castDuration;
	public string animTrigger;
	protected AudioSource audioSource;
	protected AudioClip onCastAudio;
	protected bool isMelee;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void Init(GameObject player){
		parentPlayer = player;
		if (isMelee) {
			transform.parent = player.transform;
			transform.localRotation = Quaternion.Euler (0, 0, player.GetComponent<Player> ().effectiveRotation);
			player.AddComponent<FixedJoint2D> ().connectedBody = GetComponent<Rigidbody2D> ();
		}
		player.GetComponent<Player> ().anim.SetTrigger (animTrigger);
		audioSource = gameObject.AddComponent<AudioSource> ();
		OnCast ();
	}

	protected virtual void OnCast(){
		if (onCastAudio != null) {
			audioSource.clip = onCastAudio;
			audioSource.Play ();
		}
	}

	public virtual void OnCastFinish(){
		if (isMelee) {
			Destroy (parentPlayer.GetComponent<FixedJoint2D> ());
		}
		Destroy (gameObject);
	}
}
