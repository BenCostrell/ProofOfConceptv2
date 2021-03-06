﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallop : Attack {

	public float delay;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void Init(Player player){
		base.Init (player);
		WallopTask delayHitbox = new WallopTask (player, this);
		player.taskManager.AddTask (delayHitbox);
	}

	protected override Vector3 GetDirectionHit(Player playerHit){
		return (playerHit.transform.position - parentPlayer.transform.position).normalized;
	}

    public override void SetActive()
    {
        base.SetActive();
        PlaySpecialAudio();
    }

    public override void SetInactive()
    {
        TurnOffHitbox();
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
