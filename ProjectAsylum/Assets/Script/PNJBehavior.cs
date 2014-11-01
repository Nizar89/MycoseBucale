using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PNJBehavior : MonoBehaviour {
	//Random Position
	public float _changingPositionFrequency;
	public float _walkRadius;

	private NavMeshAgent _NMAgent;
	private bool _onRandomTargetGeneration;

	//Player Detection
	public GameObject _player;
	public float _playerDetectionRadius;


	private Vector3 _pnjEyePosition;


	//PNJ States
	public enum PNJStates {MovingRandom, MovingToPlayer, NextToPlayer, AlertingOthers, MovingToAlerting}
	public PNJStates _currentState;

	// FUNCTIONS
	// Use this for initialization
	void Start () {
		// Variable Initialisation
		_pnjEyePosition = transform.FindChild("EyePosition").transform.position;
		_currentState = PNJStates.MovingRandom;
		_NMAgent = GetComponent<NavMeshAgent>();
		_onRandomTargetGeneration = true;
		//Coroutines Launching
		StartCoroutine("RandomTargetGeneration");
	}
	
	// Update is called once per frame
	void Update () {

	}

	void PlayerDetection (){
		List<Collider> tmpObjectsNextToPNJ = new List<Collider>(Physics.OverlapSphere(this.transform.position,_playerDetectionRadius));
		if (IsPlayerVisible() && tmpObjectsNextToPNJ.Contains(_player.collider)){
			_NMAgent.SetDestination(_player.transform.position);
			_currentState = PNJStates.MovingToPlayer;
		}

	}

	bool IsPlayerVisible (){
		bool tmpIsPLayerVisible = false;
		//Raycasting From this.Eye to player.Eye

		//Raycasting From this.Eye to player.Foot

		return tmpIsPLayerVisible;
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
}
