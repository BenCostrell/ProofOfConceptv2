﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pull : Attack {
	public float speed;
	public float distanceToPullTo;
	public float hitstun;
    public float positionOffset;
    private FSM<Pull> stateMachine;
    [HideInInspector]
    public Player hookedPlayer;
    [HideInInspector]
    public Rigidbody2D rb;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        stateMachine.Update();
    }

	public override void Init(Player player){
		base.Init (player);
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new FSM<Pull>(this);
        stateMachine.TransitionTo<PreFire>();
    }

    protected override Vector3 GetDirectionHit(Player playerHit){
		return GetComponent<Rigidbody2D> ().velocity.normalized;
	}

	protected override void HitPlayer(Player player){
		base.HitPlayer (player);
		Services.EventManager.Fire (new PlayerHooked (player));
	}

    public override void SetActive()
    {
        base.SetActive();
        stateMachine.TransitionTo<Extending>();
    }

    public void BeginToExtend()
    {
        float angle = parentPlayer.effectiveRotation;
        Vector3 direction = new Vector3(-Mathf.Cos(angle * Mathf.Deg2Rad), -Mathf.Sin(angle * Mathf.Deg2Rad));

        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.localPosition += positionOffset * direction;
        GetComponent<Rigidbody2D>().velocity = speed * direction;
    }

    public override void OnCastFinish()
    {
        Debug.Log("ending pull at time " + Time.time);
        Services.EventManager.Fire(new AbilityEnded(this));
    }

    public void DestroyPull()
    {
        Destroy(gameObject);
    }


    // STATES //

    private class PullState : FSM<Pull>.State
    {
        public override void OnEnter()
        {
            Services.EventManager.Register<AbilityEnded>(OnAbilityEnded);
        }

        public override void OnExit()
        {
            Services.EventManager.Unregister<AbilityEnded>(OnAbilityEnded);
        }

        protected virtual void OnAbilityEnded(AbilityEnded e)
        {
            if (e.ability == Context)
            {
                if (Context.parentPlayer.stunned) EndPull();
            }
        }

        protected virtual void EndPull()
        {
            Services.EventManager.Unregister<AbilityEnded>(OnAbilityEnded);
            Context.DestroyPull();
        }
    }

    private class PreFire : PullState
    {
    }

    private class Extending : PullState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Context.BeginToExtend();
            Services.EventManager.Register<PlayerHooked>(StartRetracting);
        }

        void StartRetracting(PlayerHooked e)
        {
            Context.hookedPlayer = e.hookedPlayer;
            TransitionTo<Retracting>();
            Services.EventManager.Unregister<PlayerHooked>(StartRetracting);
        }

        protected override void OnAbilityEnded(AbilityEnded e)
        {
            if (e.ability == Context)
            {
                EndPull();
            }
        }

        protected override void EndPull()
        {
            Services.EventManager.Unregister<PlayerHooked>(StartRetracting);
            base.EndPull();
        }
    }

    private class Retracting : PullState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Context.hookedPlayer.StopListeningForInput();
        }

        public override void Update()
        {
            Context.rb.velocity = Context.speed * 
                (Context.parentPlayer.transform.position - Context.hookedPlayer.transform.position).normalized;
            Context.hookedPlayer.rb.MovePosition(Context.transform.position);
            if (InPosition())
            {
                EndPull();
                Context.hookedPlayer.Stun(Context.hitstun);
                Context.parentPlayer.StartListeningForInput();
            }
        }

        bool InPosition()
        {
            return (Vector3.Distance(Context.parentPlayer.transform.position, Context.hookedPlayer.transform.position) 
                <= Context.distanceToPullTo);
        }

        protected override void EndPull()
        {
            Context.hookedPlayer.StartListeningForInput();
            base.EndPull();
        }
    }

}
