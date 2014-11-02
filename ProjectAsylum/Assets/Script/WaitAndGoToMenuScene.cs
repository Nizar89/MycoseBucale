using UnityEngine;
using System.Collections;

public class WaitAndGoToMenuScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine("AutoToMenu");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKey){
			Application.LoadLevel(0);
		}
	}

	IEnumerator AutoToMenu (){
		yield return new WaitForSeconds(5.0f);
		Application.LoadLevel(0);
	}
}
