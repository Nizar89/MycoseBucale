using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManagerForPlayer : MonoBehaviour {
	
	public List<AudioClip> _audioClips;

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
			//
			_audioSource.pitch = 1;
			_audioSource.clip = _audioClips[1];
			_audioSource.Play();
			break;
		case 2:
			//
			_audioSource.pitch = 1;
			_audioSource.clip = _audioClips[2];
			_audioSource.Play();			
			break;
		case 3:
			//
			_audioSource.pitch = 1;
			_audioSource.clip = _audioClips[3];
			_audioSource.Play();			
			break;
		case 4:
			//
			_audioSource.pitch = 1;
			_audioSource.clip = _audioClips[4];
			_audioSource.Play();			
			break;
		case 5:
			//
			_audioSource.pitch = 1;
			_audioSource.clip = _audioClips[5];
			_audioSource.Play();			
			break;
		case 6:
			//
			_audioSource.pitch = 1;
			_audioSource.clip = _audioClips[6];
			_audioSource.Play();			
			break;

		}
	}
	
	void RandomInAudio (List<AudioClip> list){
		_audioSource.clip = list[Random.Range(0, list.Count)];
		_audioSource.Play();
	}
}
