using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PNJBehavior : MonoBehaviour {
	//Random Position
	public float _changingPositionFrequency;
	public float _walkRadius;

	private NavMeshAgent _NMAgent;
	private bool _onRandomTargetGeneration;

	//Player Detection
	public float _pnjVisionRadius = 50.0f;
	public float _pnjVisionDistance = 10.0f;
	public float _pnjDistMinToPlayer = 2.0f;

	private GameObject _player;
	private Transform _pnjEyePosition;
	public GameObject _cadavre;

	//PNJ States
	public enum PNJStates {Unaware, Curious, Panic, Escape, Talking}
	public PNJStates _currentState;
	private Transform _destinationWhenCurious;

	// Cadavre
	private List <GameObject> _cadavreAlreadySeen;
	//Talking
	public float _timeBetweenTalks = 5.0f;
	private bool _onTalkingToPlayer;


	//Feedbacks



	private TextMesh _stateIndicator;

	// FUNCTIONS
	// Use this for initialization
	void Start () {
		// Variable Initialisation
		_cadavreAlreadySeen = new List<GameObject>();
		_player = RunAndCrouch._Player;
		_pnjEyePosition = transform.FindChild("EyePosition").transform;
		_stateIndicator = transform.FindChild("GUIState").GetComponent<TextMesh>();
		_NMAgent = GetComponent<NavMeshAgent>();
		_onRandomTargetGeneration = true;
		_onTalkingToPlayer = true;

		//First State
		_currentState = PNJStates.Unaware;
		StateManager(_currentState);
	}
	
	// Update is called once per frame
	void Update () {
		//Ste Destination Update
		if (_destinationWhenCurious != null){
			_NMAgent.SetDestination(_destinationWhenCurious.position);
		}

		//When he hear a sound

		//When he see something
		if (IsCadavreVisible() && _currentState == PNJStates.Unaware){
			_currentState = PNJStates.Curious;
			StateManager(_currentState);
		}

		// If he see the Player
		if (IsPlayerVisible() && _currentState == PNJStates.Unaware){
			_currentState = PNJStates.Curious;
			StateManager(_currentState);
			// If Monster is In

			// If Monster is Out
		}

		//When he is next to the player
		if (_currentState == PNJStates.Curious && IsNextToDestination(_destinationWhenCurious.position)){
			_currentState = PNJStates.Talking;
			StateManager(_currentState);
			//EatenByMonster
		}

		// When He is talking
		if (_currentState == PNJStates.Talking && IsPlayerVisible() == false){
			_currentState = PNJStates.Unaware;
			StateManager(_currentState);
		}

		if (IsPlayerVisible() && BehaviorMonster._monster._visualState == BehaviorMonster.VisualState.Degeu)
		{
			_currentState = PNJStates.Panic;
			StateManager(_currentState);
		}

	}

	void StateManager (PNJStates state){
		switch (state){
			case PNJStates.Unaware:
				StartCoroutine("RandomTargetGeneration");
				_stateIndicator.text = "Unaware";
			break;
			case PNJStates.Curious:
				StopCoroutine("RandomTargetGeneration");
				_NMAgent.SetDestination(_destinationWhenCurious.position);
				_stateIndicator.text = "Curious";
			break;
			case PNJStates.Escape:
				StopCoroutine("RandomTargetGeneration");
			break;
			case PNJStates.Panic:
				StopCoroutine("RandomTargetGeneration");
			break;
			case PNJStates.Talking:
				StartCoroutine("Talking");
				_NMAgent.SetDestination(_destinationWhenCurious.position);
			break;
		}
	}

	bool IsPlayerVisible (){
		bool tmpIsPLayerVisible = false;
		Vector3 tmpPNJToPlayerHead = _player.transform.FindChild("Head").position - _pnjEyePosition.position;
		Vector3 tmpPNJToPlayerFoot = _player.transform.FindChild("Foot").position - _pnjEyePosition.position;

		//Raycasting From this.Eye to player.Eye
		Ray Charles = new Ray(_pnjEyePosition.position, tmpPNJToPlayerHead);
		RaycastHit hit;
		Physics.Raycast(Charles, out hit, _pnjVisionDistance);
		if (hit.collider != null && hit.collider.gameObject == _player.gameObject && Vector3.Angle(_pnjEyePosition.forward, tmpPNJToPlayerHead) <= _pnjVisionRadius){
			tmpIsPLayerVisible = true;
			_destinationWhenCurious = _player.transform;
		}
		//Raycasting From this.Eye to player.Foot
		if (!tmpIsPLayerVisible){
			Ray Manzarek = new Ray(_pnjEyePosition.position, tmpPNJToPlayerFoot);
			RaycastHit hit2;
			Physics.Raycast(Manzarek, out hit2, _pnjVisionDistance);
			if (hit.collider != null && hit.collider.gameObject == _player.gameObject && Vector3.Angle(_pnjEyePosition.forward, tmpPNJToPlayerFoot) <= _pnjVisionRadius){
				tmpIsPLayerVisible = true;
				_destinationWhenCurious = _player.transform;
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
				_destinationWhenCurious = tmpCadavre.transform;
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
			yield return new WaitForSeconds(_changingPositionFrequency);
		}
	}

	IEnumerator Talking (){
		while (_onTalkingToPlayer){
			//LaunchtalkingFeedback
			yield return new WaitForSeconds(_timeBetweenTalks);
		}
	}
}
