using UnityEngine;
using System.Collections;

public class BehaviorMonster : MonoBehaviour 
{
	float _aggroZone = 10f;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	IEnumerator HandlePnj()
	{
		while (true)
		{
			Collider[] hits = Physics.OverlapSphere(this.transform.position, _aggroZone);

			foreach(Collider hit in hits)
			{

			}
		}
	}
}
