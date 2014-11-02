using UnityEngine;
using System.Collections;

public class InfectedBehavior : MonoBehaviour 
{

	public float _durationTransformation = 5f;
	public float _durationFeed = 10f;
	public float _walkRadius = 20f;

	public float _speedWalk = 20f;
	public float _speedAttack = 40f;
	public float _distanceVision = 90f;

	public Collider _triggerAttack;

	private MyFsm _fsm = new MyFsm();
	private Vector3 _destination = new Vector3();
	private NavMeshAgent _navMeshAgent;
	private GameObject _targetToKill = null;
	private Transform _pnjEyePosition;

	private Vector3 _target = new Vector3();

	private enum State
	{
		Transformation,
		Hunting,
		Attack,
		Feeding
	}

	class MyFsm : Fsm
	{
		public InfectedBehavior m_controller;
		
		/// Virtual method called when state enter, update or leave occur
		public override void OnStateUpdate(FsmStateEvent eEvent, uint a_oState)
		{
			switch ((State)a_oState)
			{
				case State.Transformation:
					m_controller.StateTransformation(eEvent);
					break;
				case State.Hunting:
					m_controller.StateHunting(eEvent);
					break;
				case State.Attack:
					m_controller.StateAttack(eEvent);
					break;
				case State.Feeding:
					m_controller.StateFeeding(eEvent);
					break;

			}
		}
	}

	void Awake()
	{
		_fsm.m_controller = this;
	}

	// Use this for initialization
	void Start () 
	{
		_navMeshAgent = this.GetComponent<NavMeshAgent>();
		_pnjEyePosition = transform.FindChild("EyePosition").transform;

		Destroy(this.GetComponent<PNJBehavior>());

		SetState(State.Transformation);
	}
	
	// Update is called once per frame
	void Update () 
	{
		_fsm.UpdateFsm(Time.deltaTime);

	}

	void SetState(State newState)
	{
		_fsm.SetRequestedState((uint)newState);
	}

	void StateTransformation(FsmStateEvent eEvent)
	{
		switch (eEvent)
		{
			case FsmStateEvent.eEnter:
			{
				//Play anim
				_navMeshAgent.Stop();
				break;
			}
			case FsmStateEvent.eUpdate:
			{
				if (_durationTransformation <= _fsm.GetFsmStateTime())
				{
					SetState(State.Hunting);
				}
				break;
			}
		}
	}

	void StateHunting(FsmStateEvent eEvent)
	{
		switch (eEvent)
		{
			case FsmStateEvent.eEnter:
			{
				//Play anim
				RandomTargetGeneration();
				_navMeshAgent.SetDestination(_destination);
				break;
			}
			case FsmStateEvent.eUpdate:
			{
				if (Vector3.Distance(this.transform.position, _destination) < 0.5f)
				{
					//launchAnim, then set RandomTargetGeneration at the end of it
				}
				HandleRushWhenSeePlayer();
				break;
			}
		}
	}

	void StateAttack(FsmStateEvent eEvent)
	{
		switch (eEvent)
		{
			case FsmStateEvent.eEnter:
			{
				//Play anim
				_navMeshAgent.speed = _speedAttack;
				_triggerAttack.enabled = true;
				break;
			}
			case FsmStateEvent.eUpdate:
			{
				_navMeshAgent.SetDestination(_target);
				if (Vector3.Distance(this.transform.position, _target) < 0.5f)
				{
					SetState(State.Hunting);
				}
				else if (_targetToKill != null)
				{
					_targetToKill.SendMessage("Death");
					SetState(State.Feeding);
				}
				break;
			}
			case FsmStateEvent.eLeave:
			{
				_navMeshAgent.speed = _speedWalk;
				_triggerAttack.enabled = false;
				_targetToKill = null;
				break;
			}

		}
	}

	void StateFeeding(FsmStateEvent eEvent)
	{
		switch (eEvent)
		{
			case FsmStateEvent.eEnter:
			{
				//LaunchAnim
				_navMeshAgent.Stop();
				break;
			}
			case FsmStateEvent.eUpdate:
			{
				if (_fsm.GetFsmStateTime() > _durationFeed)
				{
					SetState(State.Hunting);
				}
				break;
			}
		}
	}

	public void ScreamResponse(Vector3 position)
	{
		if (this.enabled == true)
		{
			_target = position;
			SetState(State.Attack);
		}

	}

	public Vector3 RandomTargetGeneration () //Call by animation
	{
		// Generate Random Position On Navmesh
		Vector3 randomDirection = Random.insideUnitSphere * _walkRadius;
		randomDirection += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, _walkRadius, 1);
		return (hit.position);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "PNJ" || other.tag == "Player")
		{
			_targetToKill = other.collider.gameObject;
		}
	}

	void HandleRushWhenSeePlayer()
	{
		if (IsPlayerVisible())
		{
			_target = RunAndCrouch._Player.transform.position;
			SetState(State.Attack);
		}
	}

	bool IsPlayerVisible ()
	{
		bool tmpIsPLayerVisible = false;
		

		
		Vector3 tmpPNJToPlayerHead = RunAndCrouch._Player.transform.FindChild("Head").position - _pnjEyePosition.position;
		Vector3 tmpPNJToPlayerFoot = RunAndCrouch._Player.transform.FindChild("Foot").position - _pnjEyePosition.position;
		Vector3 tmpPNJToPlayer = RunAndCrouch._Player.transform.position - new Vector3(_pnjEyePosition.position.x, RunAndCrouch._Player.transform.position.y, _pnjEyePosition.position.z);
		Debug.DrawRay(_pnjEyePosition.position, tmpPNJToPlayer * 5, Color.red);
		//Raycasting From this.Eye to player.Eye
		Ray Charles = new Ray(_pnjEyePosition.position, tmpPNJToPlayerHead);
		RaycastHit hit;
		Physics.Raycast(Charles, out hit, _distanceVision);
		if (hit.collider != null && hit.collider.gameObject == RunAndCrouch._Player && Vector3.Angle(_pnjEyePosition.forward, tmpPNJToPlayer) <= _distanceVision)
		{
			tmpIsPLayerVisible = true;
		}
		//Raycasting From this.Eye to player.Foot
		if (!tmpIsPLayerVisible)
		{
			Ray Manzarek = new Ray(_pnjEyePosition.position, tmpPNJToPlayerFoot);
			RaycastHit hit2;
			Physics.Raycast(Manzarek, out hit2, _distanceVision);
			if (hit.collider != null && hit.collider.gameObject == RunAndCrouch._Player.gameObject && Vector3.Angle(_pnjEyePosition.forward, tmpPNJToPlayer) <= _distanceVision)
			{
				tmpIsPLayerVisible = true;
			}
		}
		return tmpIsPLayerVisible;
	}

	public void Death()
	{
		Debug.Log ("Nope.avi");
	}
}
