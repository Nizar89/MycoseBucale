using UnityEngine;
using System.Collections;

public class AlarmBehavior : MonoBehaviour {

	public int _limitToLaunchAlarm;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void ReduceAlarmLimit (){
		_limitToLaunchAlarm --;
		if (_limitToLaunchAlarm <=0){

		}
	}

	void AlarmLaunched (){

	}
}
