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
		if (!_audioSource.isPlaying){
			_audioSource.Play();
		}
	}

	public void PlayAudio (int index){
		switch(index){
		case 1:
			//Marche
			if (_audioSource.clip != _audioClips[0]){
				_audioSource.pitch = 1;
				_audioSource.clip = _audioClips[0];
				_audioSource.Play();
			}
		break;
		case 2:
			//Course
			if (_audioSource.clip != _audioClips[0]){
				_audioSource.pitch = 2;
				_audioSource.clip = _audioClips[0];
				_audioSource.Play();
			}
			break;
		case 3:
			//Cri
			if (_audioSource.clip != _audioClips[1]){
				_audioSource.pitch = 1;
				_audioSource.clip = _audioClips[1];
				_audioSource.Play();
			}
			break;
		case 4:
			//Voices
			_audioSource.pitch = 1;
			RandomVoice();
			break;
		//INFECTE
		case 5:
			//Marche
			if (_audioSource.clip != _audioClips[2]){
				_audioSource.pitch = 1;
				_audioSource.clip = _audioClips[2];
				_audioSource.Play();
			}
			break;
		case 6:
			//Course
		if (_audioSource.clip != _audioClips[2]){
			_audioSource.pitch = 2;
			_audioSource.clip = _audioClips[2];
			_audioSource.Play();
		}
		break;
		case 7:
			//Cri d'infectation
			if (_audioSource.clip != _audioClips[3]){
				_audioSource.pitch = 1;
				_audioSource.clip = _audioClips[3];
				_audioSource.Play();
			}
		break;
	}
}
	
	void RandomVoice (){
		_audioSource.clip = _voices[Random.Range(0, _voices.Count)];
		_audioSource.Play();
	}
}
