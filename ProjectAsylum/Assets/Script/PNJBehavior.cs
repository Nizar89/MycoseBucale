﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PNJBehavior : MonoBehaviour {
	public bool _IsVIP;
	public static bool _isVIPDead;
	public Material _vipMat;
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
	public GameObject _cadavreVIP;

	public float _distanceToHearLightSound;
	public float _distanceToHearMediumSound;

	//PNJ States
	public enum PNJStates {Unaware, Curious, Panic, Escape, Talking}
	public PNJStates _currentState;
	private Vector3 _destinationWhenCurious;

	// Cadavre
	private List <GameObject> _cadavreAlreadySeen;
	//Talking
	public float _timeBetweenTalks = 5.0f;
	private bool _onTalkingToPlayer;

	//Panic & Escape
	public float _panicScreamRange;
	public static GameObject _theExitDoor;
	private Vector3 _exitDoor;
	//Feedbacks
	private TextMesh _stateIndicator;
	public Animator _animator;

	// FUNCTIONS
	// Use this for initialization
	void Start () {
		// Variable Initialisation
		_isVIPDead = false;
		_cadavreAlreadySeen = new List<GameObject>();
		_player = RunAndCrouch._Player;
		_pnjEyePosition = transform.FindChild("EyePosition").transform;
		_stateIndicator = transform.FindChild("GUIState").GetComponent<TextMesh>();
		_NMAgent = GetComponent<NavMeshAgent>();
		_onRandomTargetGeneration = true;
		_onTalkingToPlayer = true;
		_exitDoor = GameObject.Find("Porte de Sortie").transform.position;
		_theExitDoor = GameObject.Find("Porte de Sortie");
		_animator = this.GetComponentInChildren<Animator>();
		//
		if (_IsVIP){
			this.renderer.material = _vipMat;
		}
		//First State
		_currentState = PNJStates.Unaware;
		StateManager(_currentState);

	}
	
	// Update is called once per frame
	void Update () {
		
		if ( _currentState == PNJStates.Talking){
			this.transform.LookAt(_player.transform.position);
		}



		//Ste Destination Update
		if (_destinationWhenCurious != null && _currentState != PNJStates.Unaware && _currentState != PNJStates.Panic && _currentState != PNJStates.Escape){
			_NMAgent.SetDestination(_destinationWhenCurious);
		}

		//When he see something
		if (IsCadavreVisible() && _currentState == PNJStates.Unaware){
			print ("See A cadavre and is Unaware");
			_currentState = PNJStates.Curious;
			StateManager(_currentState);
		}

		// If he see the Player
		if (IsPlayerVisible() && _currentState == PNJStates.Unaware){
			print ("See the player and is Unaware");
			_currentState = PNJStates.Curious;
			StateManager(_currentState);
			// If Monster is In

			// If Monster is Out
		}

		//When he is next to the player
		if (_currentState == PNJStates.Curious && IsNextToDestination(_player.transform.position)){
			_currentState = PNJStates.Talking;
			StateManager(_currentState);
			//EatenByMonster
		}

		if (_currentState == PNJStates.Unaware 
		    && BehaviorMonster._monster._typeOfSound == BehaviorMonster.TypeOfSound.Small 
		    && Vector3.Distance(this.transform.position,_player.transform.position) <= _distanceToHearLightSound){
			Vector3 tmpV3 = new Vector3 ( this.transform.position.x, this.transform.position.y, this.transform.position.z);
			ScreamResponse (tmpV3);
			print ("Is Unaware, ear a sound Small");

		}

		if (_currentState == PNJStates.Unaware 
		    && BehaviorMonster._monster._typeOfSound == BehaviorMonster.TypeOfSound.Medium 
		    && Vector3.Distance(this.transform.position,_player.transform.position) <= _distanceToHearMediumSound){
			ScreamResponse (this.transform.position);
			print ("Is Unaware, ear a sound BIG");
		}

		// When He is talking
		if (_currentState == PNJStates.Talking && IsNextToDestination(_destinationWhenCurious)){
			_NMAgent.Stop();
		}

		if (_currentState == PNJStates.Talking && !IsNextToDestination(_destinationWhenCurious)){
			_NMAgent.SetDestination(_destinationWhenCurious);
			print ("Stop To Go");
		}

		if (_currentState == PNJStates.Talking && IsPlayerVisible() == false){
			print ("talking to unaware");
			_currentState = PNJStates.Unaware;
			StateManager(_currentState);
		}

		if (_currentState == PNJStates.Escape && IsNextToDestination(_exitDoor)){
			print ("Hasta la vista baby");
			if (_IsVIP){
				GameObject.Find("Porte de Sortie").GetComponent<AlarmBehavior>().AlarmLaunched();
			}else{
				GameObject.Find("Porte de Sortie").GetComponent<AlarmBehavior>().ReduceAlarmLimit();
			}
			Destroy(this.gameObject);
		}

		if ((_currentState == PNJStates.Curious 
		    || _currentState == PNJStates.Talking)
		    && IsPlayerVisible() 
		    && BehaviorMonster._monster._visualState == BehaviorMonster.VisualState.Degeu)
		{
			_currentState = PNJStates.Panic;
			StateManager(_currentState);
		}

	}

	void StateManager (PNJStates state){
		switch (state){
			case PNJStates.Unaware:
				_NMAgent.Stop();
				StopCoroutine("Talking");
				StartCoroutine("RandomTargetGeneration");
				_stateIndicator.text = "Unaware";
				_animator.SetBool("Walk", true);
				_animator.SetInteger("Speak", 12);

			break;
			case PNJStates.Curious:
				StopCoroutine("Talking");
				StopCoroutine("RandomTargetGeneration");
				_NMAgent.SetDestination(_destinationWhenCurious);
				_stateIndicator.text = "Curious";
				_animator.SetBool("Walk", true);
				_animator.SetInteger("Speak", 12);
			break;
			case PNJStates.Escape:
				StopCoroutine("Talking");
				StopCoroutine("RandomTargetGeneration");
				_destinationWhenCurious = _exitDoor;
				_NMAgent.SetDestination(_destinationWhenCurious);
				_stateIndicator.text = "Escape";
				_animator.SetBool("Run", true);
	
			break;
			case PNJStates.Panic:
				StopCoroutine("Talking");
				StopCoroutine("RandomTargetGeneration");
				_NMAgent.Stop();
				_stateIndicator.text = "Panic";
				StartCoroutine("Panicking");
				_animator.SetTrigger("Shout");

			break;
			case PNJStates.Talking:
				StartCoroutine("Talking");
				_NMAgent.SetDestination(_destinationWhenCurious);
				_stateIndicator.text = "Talking";
				_animator.SetBool("Walk", false);
				_animator.SetInteger("Speak", Random.Range(1,3));
				
			break;
		}
	}

	public void ScreamResponse (Vector3 cryPosition){

		_destinationWhenCurious = new Vector3(cryPosition.x, this.transform.position.y, cryPosition.z);
		_currentState = PNJStates.Curious;
		StateManager(_currentState);
	}

	public void Death (){
		Destroy(this.collider);
		if (_IsVIP){
			_isVIPDead = true;
			Instantiate(_cadavreVIP, this.transform.position, Quaternion.identity);
		}else{
			Instantiate(_cadavre, this.transform.position, Quaternion.identity);
		}
		Destroy(this.gameObject);
		//At the end of the animation, launch destroy
	}
	public void PNJToInfected (){
		_stateIndicator.text = "Infected";
		this.GetComponent<InfectedBehavior>().enabled = true;
	}

	bool IsPlayerVisible (){
		bool tmpIsPLayerVisible = false;
		Vector3 tmpPNJToPlayerHead = _player.transform.FindChild("Head").position - _pnjEyePosition.position;
		Vector3 tmpPNJToPlayerFoot = _player.transform.FindChild("Foot").position - _pnjEyePosition.position;
		Vector3 tmpPNJToPlayer = _player.transform.position - new Vector3(_pnjEyePosition.position.x, _player.transform.position.y, _pnjEyePosition.position.z);
		//Raycasting From this.Eye to player.Eye
		Ray Charles = new Ray(_pnjEyePosition.position, tmpPNJToPlayerHead);
		RaycastHit hit;
		Physics.Raycast(Charles, out hit, _pnjVisionDistance);
		if (hit.collider != null && hit.collider.gameObject == _player && Vector3.Angle(_pnjEyePosition.forward, tmpPNJToPlayer) <= _pnjVisionRadius){
			tmpIsPLayerVisible = true;
			_destinationWhenCurious = _player.transform.position;
		}
		//Raycasting From this.Eye to player.Foot
		if (!tmpIsPLayerVisible){
			Ray Manzarek = new Ray(_pnjEyePosition.position, tmpPNJToPlayerFoot);
			RaycastHit hit2;
			Physics.Raycast(Manzarek, out hit2, _pnjVisionDistance);
			if (hit.collider != null && hit.collider.gameObject == _player.gameObject && Vector3.Angle(_pnjEyePosition.forward, tmpPNJToPlayer) <= _pnjVisionRadius){
				tmpIsPLayerVisible = true;
				_destinationWhenCurious = _player.transform.position;
			}
		}
		return tmpIsPLayerVisible;
	}

	bool IsNextToDestination (Vector3 destination){
		bool tmpIsNextToPlayer = false;
		if (Vector3.Distance(this.transform.position, destination) <= _pnjDistMinToPlayer){
			tmpIsNextToPlayer = true;
		}

		return tmpIsNextToPlayer;
	}

	bool IsCadavreVisible (){
		bool tmpIsCadavreVisible = false;
		GameObject tmpCadavre = null;
		List<Collider> _objectsNearPNJ = Physics.OverlapSphere(this.transform.position, _pnjVisionDistance).ToList();
		foreach (Collider objects in _objectsNearPNJ){
			if (objects.gameObject == _cadavre.gameObject && _cadavreAlreadySeen.Contains(objects.gameObject) == false){
				tmpCadavre = objects.gameObject;
				_cadavreAlreadySeen.Add(tmpCadavre);
			}
		}
		if (tmpCadavre != null){
			Vector3 tmpEyeToCadavre = tmpCadavre.transform.position - _pnjEyePosition.position;
			//Raycasting From this.Eye to player.Eye
			Ray Charles = new Ray(_pnjEyePosition.position, tmpEyeToCadavre);
			RaycastHit hit;
			Physics.Raycast(Charles, out hit, _pnjVisionDistance);
			if (hit.collider != null && hit.collider.gameObject == _player.gameObject && Vector3.Angle(_pnjEyePosition.forward, tmpEyeToCadavre) <= _pnjVisionRadius){
				tmpIsCadavreVisible = true;
				_destinationWhenCurious = tmpCadavre.transform.position;
			}
		}

		return tmpIsCadavreVisible;
	}


	// COROUTINES
	IEnumerator RandomTargetGeneration (){
		while (_onRandomTargetGeneration){
			// Generate Random Position On Navmesh
			Vector3 randomDirection = Random.insideUnitSphere * _walkRadius;
			randomDirection += transform.position;
			NavMeshHit hit;
			NavMesh.SamplePosition(randomDirection, out hit, _walkRadius, 1);
			Vector3 finalPosition = hit.position;
			_NMAgent.SetDestination(finalPosition);
			//Waiting
			print ("Change Direction");
			yield return new WaitForSeconds(_changingPositionFrequency);
		}
	}

	IEnumerator Talking (){
		while (_onTalkingToPlayer){
			//LaunchtalkingFeedback
			yield return new WaitForSeconds(_timeBetweenTalks);
		}
	}

	IEnumerator Panicking (){
		// Scream Animation
		// Scream Sound
		List<Collider> _objectsHearingScream = Physics.OverlapSphere(this.transform.position, _panicScreamRange).ToList();
		_objectsHearingScream.Remove(this.collider);
		foreach (Collider objects in _objectsHearingScream){
			if (objects.tag == "PNJ" && (objects.GetComponent<PNJBehavior>()._currentState == PNJStates.Curious || objects.GetComponent<PNJBehavior>()._currentState == PNJStates.Unaware)){
				Vector3 tmpV3 = new Vector3 (this.transform.position.x, this.transform.position.y, this.transform.position.z);
				objects.gameObject.SendMessage("ScreamResponse", tmpV3);
			}
		}
		yield return new WaitForSeconds(Random.Range(2.0f, 5.0f));
		_currentState = PNJStates.Escape;
		StateManager (_currentState);

	}
}
