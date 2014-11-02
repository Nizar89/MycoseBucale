using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlarmBehavior : MonoBehaviour {

	public int _limitToLaunchAlarm;
	public List<GameObject> _doors;
	public List<GameObject> _alarmsLights;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Space)){
			_limitToLaunchAlarm = 1;
			ReduceAlarmLimit ();
		}
	}

	public void ReduceAlarmLimit (){
		_limitToLaunchAlarm --;
		if (_limitToLaunchAlarm <=0){
			AlarmLaunched ();
		}
	}

	void AlarmLaunched (){

		foreach (GameObject door in _doors){
			door.GetComponent<Animator>().Play("Open");
			door.GetComponent<BoxCollider>().enabled = true;
			door.GetComponent<NavMeshObstacle>().enabled = true;
		}

		foreach (GameObject alarm in _alarmsLights){
			alarm.GetComponent<Light>().enabled = true;
			alarm.GetComponent<Animation>().Play("LightAnim");

		}



	}
}
