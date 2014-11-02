using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PNJBehaviorV2 : MonoBehaviour 
{
	//Random Position
	public float _changingPositionFrequency;
	public float _walkRadius;
	
	private NavMeshAgent _NMAgent;
	private bool _onRandomTargetGeneration;
	
	//Player Detection
	public float _pnjVisionRadius = 50.00f;
	public float _pnjVisionDistance = 10.0f;
	public float _pnjDistMinToPlayer = 2.0f;
	
	private GameObject _player;
	private Transform _pnjEyePosition;
	public GameObject _cadavre;
	
	public float _distanceToHearLightSound;
	public float _distanceToHearMediumSound;
	
	//PNJ States
	public enum State {Unaware, Curious, Panic, Escape, Talking}
	private Vector3 _destinationWhenCurious;
	
	// Cadavre
	private List <GameObject> _cadavreAlreadySeen;

	//Talking
	public float _timeBetweenTalks = 5.0f;
	private bool _onTalkingToPlayer;
	
	//Panic & Escape
	public float _panicScreamRange;
	private Vector3 _exitDoor;
	//Feedbacks
	private TextMesh _stateIndicator;
	private Animator _animator;


	private MyFsm _fsm = new MyFsm();

	class MyFsm : Fsm
	{
		public PNJBehaviorV2 m_controller;
		
		/// Virtual method called when state enter, update or leave occur
		public override void OnStateUpdate(FsmStateEvent eEvent, uint a_oState)
		{
			switch ((State)a_oState)
			{
			case State.Unaware:
				m_controller.StateUnaware(eEvent);
				break;
			case State.Curious:
				m_controller.StateCurious(eEvent);
				break;
			}
		}
	}


	void Awake()
	{
		_fsm.m_controller = this;
	}

	void Start () 
	{
		_NMAgent = this.GetComponent<NavMeshAgent>();
		_animator = this.GetComponent<Animator>();
		SetState(State.Unaware);
	}
	
	// Update is called once per frame
	void Update () 
	{
		_fsm.UpdateFsm(Time.deltaTime);
	}


	void StateUnaware(FsmStateEvent eEvent)
	{
		switch(eEvent)
		{
			case FsmStateEvent.eEnter:
			{
				
				_NMAgent.SetDestination(RandomTargetGeneration());
				_animator.SetBool("Walk", true);
				break;
			}
			case FsmStateEvent.eUpdate:
			{
				if (_NMAgent.remainingDistance < 0.2f)
				{
					_NMAgent.SetDestination(RandomTargetGeneration());
				}
				if (IsPlayerVisible() || (BehaviorMonster._monster._typeOfSound == BehaviorMonster.TypeOfSound.Medium))
				{
					SetState(State.Curious);
				}
				break;
			}
		}
	}

	void StateCurious(FsmStateEvent eEvent)
	{

	}

	void SetState(State newState)
	{
		_fsm.SetRequestedState((uint)newState);
	}

	bool IsPlayerVisible ()
	{
		bool tmpIsPLayerVisible = false;

		Vector3 tmpPNJToPlayerHead = RunAndCrouch._Player.transform.FindChild("Head").position - _pnjEyePosition.position;
		Vector3 tmpPNJToPlayerFoot = RunAndCrouch._Player.transform.FindChild("Foot").position - _pnjEyePosition.position;
		Vector3 tmpPNJToPlayer = RunAndCrouch._Player.transform.position - new Vector3(_pnjEyePosition.position.x, RunAndCrouch._Player.transform.position.y, _pnjEyePosition.position.z);
		//Raycasting From this.Eye to player.Eye
		Ray Charles = new Ray(_pnjEyePosition.position, tmpPNJToPlayerHead);
		RaycastHit hit;
		Physics.Raycast(Charles, out hit, _pnjVisionDistance);
		if (hit.collider != null && hit.collider.gameObject == RunAndCrouch._Player && Vector3.Angle(_pnjEyePosition.forward, tmpPNJToPlayer) <= _pnjVisionDistance)
		{
			tmpIsPLayerVisible = true;
		}
		//Raycasting From this.Eye to player.Foot
		if (!tmpIsPLayerVisible)
		{
			Ray Manzarek = new Ray(_pnjEyePosition.position, tmpPNJToPlayerFoot);
			RaycastHit hit2;
			Physics.Raycast(Manzarek, out hit2, _pnjVisionDistance);
			if (hit.collider != null && hit.collider.gameObject == RunAndCrouch._Player.gameObject && Vector3.Angle(_pnjEyePosition.forward, tmpPNJToPlayer) <= _pnjVisionDistance)
			{
				tmpIsPLayerVisible = true;
			}
		}
		return tmpIsPLayerVisible;
	}

	public void Death ()
	{
		Instantiate(_cadavre, this.transform.position, Quaternion.identity);
		Destroy(this.gameObject);
		//At the end of the animation, launch destroy
	}

	public void PNJToInfected ()
	{
		this.GetComponent<InfectedBehavior>().enabled = true;
	}

	public Vector3 RandomTargetGeneration () //Call by animation
	{
		// Generate Random Position On Navmesh
		Vector3 randomDirection = Random.insideUnitSphere * _walkRadius;
		randomDirection += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, _walkRadius, 1);
		return(hit.position);
		
	}
}
