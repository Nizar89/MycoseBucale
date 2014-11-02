using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManagerForPlayer : MonoBehaviour {
	
	public List<AudioClip> _audioClips;
	public List<AudioClip> _audioVoice;
	public List<Sprite> _subtitles;

	public SpriteRenderer _sprite;

	private AudioSource _audioSource;
	// Use this for initialization
	void Start () {
		_audioSource = this.GetComponent<AudioSource>();
		StartCoroutine(Marcel());
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
			//
			if (_audioSource.clip != _audioClips[0]){
				_audioSource.pitch = 1;
				_audioSource.clip = _audioClips[0];
				_audioSource.Play();
			}
				break;
		case 2:
			//
			if (_audioSource.clip != _audioClips[1]){
			_audioSource.pitch = 1;
			_audioSource.clip = _audioClips[1];
			_audioSource.Play();
			}
			break;
		case 3:
			//
			if (_audioSource.clip != _audioClips[2]){
			_audioSource.pitch = 1;
			_audioSource.clip = _audioClips[2];
			_audioSource.Play();
			}
			break;
		case 4:
			//
			if (_audioSource.clip != _audioClips[3]){
			_audioSource.pitch = 1;
			_audioSource.clip = _audioClips[3];
			_audioSource.Play();
			}
			break;
		case 5:
			//
			if (_audioSource.clip != _audioClips[4]){
			_audioSource.pitch = 1;
			_audioSource.clip = _audioClips[4];
			_audioSource.Play();
			}
			break;
		case 6:
			//
			if (_audioSource.clip != _audioClips[5]){
			_audioSource.pitch = 1;
			_audioSource.clip = _audioClips[5];
			_audioSource.Play();
			}
			break;

		}
	}
	
	void RandomInAudio (List<AudioClip> list){
		_audioSource.clip = list[Random.Range(0, list.Count)];
		_audioSource.Play();
	}

	IEnumerator Marcel()
	{
		AudioSource JeanMichel = this.gameObject.AddComponent("AudioSource") as AudioSource;
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(5,10));
			//JeanMichel.clip =  _audioVoice[Random.Range(0,_audioVoice.Count)];
			_sprite.enabled = true;
			_sprite.sprite =  _subtitles[Random.Range(0,_subtitles.Count)];
			yield return new WaitForSeconds(5f);
			_sprite.enabled = false;
			yield return new WaitForSeconds(Random.Range(10,20));
		}


	}
}
