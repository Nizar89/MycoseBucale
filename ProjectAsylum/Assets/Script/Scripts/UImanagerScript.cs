using UnityEngine;
using System.Collections;

public class UImanagerScript : MonoBehaviour 
{
	public void StartGame()
	{
		//Application.LoadLevel();
		Application.LoadLevel(1);
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
