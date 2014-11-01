using UnityEngine;
using System.Collections;

public class BehaviorMonster : MonoBehaviour 
{
	public float _aggroZone = 10f;
	public float _distanceAttack = 5f;
	public float _durationDigestion = 30f;
	public float _durationBeforeDeath = 120f;
	public int _angleVision = 50;

	private GameObject _target;
	private MyFsm _fsm = new MyFsm();

	private enum State
	{
		Hunger,
		Disgestion
	}

	class MyFsm : Fsm
	{
		public BehaviorMonster m_controller;

		/// Virtual method called when state enter, update or leave occur
		public override void OnStateUpdate(FsmStateEvent eEvent, uint a_oState)
		{
			switch ((State)a_oState)
			{
			case State.Hunger:
				m_controller.StateHunger(eEvent);
				break;
			case State.Disgestion:
				m_controller.StateDisgestion(eEvent);
				break;
			}
		}
	}
	// Use this for initialization
	void Start () 
	{
		_fsm.m_controller = this;
		SetState(State.Disgestion);
		StartCoroutine(HandlePnj());
	}
	
	// Update is called once per frame
	void Update () 
	{
		_fsm.UpdateFsm(Time.deltaTime);
	}

	void StateHunger(FsmStateEvent eEvent)
	{
		switch(eEvent)
		{
			case FsmStateEvent.eEnter:
			{
				//Use Animator
				break;
			}
			case FsmStateEvent.eUpdate:
			{
				//CalculateTime
				//Send time to Animator?
				if (_fsm.GetFsmStateTime() > _durationBeforeDeath)
				{
					Death();
				}
				break;
			}
		}
	}

	void StateDisgestion(FsmStateEvent eEvent)
	{
		switch(eEvent)
		{
			case FsmStateEvent.eEnter:
			{
				//Use Animator
				break;
			}
			case FsmStateEvent.eUpdate:
			{
				//CalculateTime
				if (_fsm.GetFsmStateTime() > _durationDigestion)
				{
					SetState(State.Hunger);
				}
				break;
			}
		}
	}


	void SetState(State newState)
	{
		_fsm.SetRequestedState((uint)newState);
	}


	void Death()
	{
		Debug.Log("DIE MOZERFUKER");
	}

	//Coroutine
	IEnumerator HandlePnj()
	{
		while (true)
		{
			Collider[] hits = Physics.OverlapSphere(this.transform.position, _aggroZone);

			foreach(Collider hit in hits)
			{
				if (hit.tag == "PNJ" && hit.renderer.isVisible)
				{
					Vector3 direction = hit.transform.FindChild("EyePosition").transform.position - this.transform.position;
					RaycastHit hitage;
					if (Physics.Raycast(this.transform.position, direction,out hitage))
					{
						if (hitage.collider == hit) //If you can see the eye of the pnj
						{
							_target = hitage.collider.gameObject;
							if (Vector3.Distance(this.transform.position, hit.transform.position) <= _distanceAttack) //If you are close enough to attack
							{
								if (Vector3.Angle(this.transform.forward,direction) < _angleVision) //If angle is good
								{
									Debug.Log("AttackPnj"); //LaunchAttack on _target
								}
							}
							else
							{
								Debug.Log("Feedback pré-attaque"); //Launch feedback before attack
							}
						}
					}
				}
			}
			yield return new WaitForSeconds(0.2f);
		}
	}
}
