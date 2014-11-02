using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundMangerForPNJ : MonoBehaviour {

	public List<AudioClip> _audioClips;
	public List<AudioClip> _voices;

	private AudioSource _audioSource;
	// Use this for initialization
	void Start () {
		_audioSource = this.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlayAudio (int index){
		switch(index){
		case 1:
			//Marche
			_audioSource.pitch = 1;
			_audioSource.clip = _audioClips[1];
			_audioSource.Play();
		break;
		case 2:
			//Course
			_audioSource.pitch = 2;
			_audioSource.clip = _audioClips[1];
			_audioSource.Play();			
			break;
		case 3:
			//Cri
			_audioSource.pitch = 1;
			_audioSource.clip = _audioClips[3];
			_audioSource.Play();			
			break;
		case 4:
			//Voices
			_audioSource.pitch = 1;
			RandomVoice();
			break;
		}
	}

	void RandomVoice (){
		_audioSource.clip = _voices[Random.Range(0, _voices.Count)];
		_audioSource.Play();
	}
}
