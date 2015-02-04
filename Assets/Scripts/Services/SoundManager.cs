using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
	public static SoundManager Singleton;

	[HideInInspector]
	public List<AudioClip> MyAudioClips;

	void Start()
	{
		Singleton = this;
		MyAudioClips = new List<AudioClip> ();

		Properties _myProperties = Properties.Singleton;

		if (_myProperties == null)
			_myProperties = GameObject.FindGameObjectWithTag("GameController").GetComponent<Properties>();

		foreach (string audioName in _myProperties.SoundFileNames) 
				MyAudioClips.Add (Resources.Load<AudioClip> (Properties.SoundsFolderName + "/" + audioName));
	}

	public static AudioClip GetClip(int index)
	{
		return SoundManager.Singleton.MyAudioClips [index];
	}

	public static void PlayClipAt(AudioClip clip, Vector3 pos, float Volume, float MinDistance, float MaxDistance, float DopplerLevel = 0f)
	{
		GameObject _myGameObject = new GameObject("TempAudio");
		_myGameObject.transform.position = pos;
		AudioSource _audioSource = _myGameObject.AddComponent<AudioSource>();
		_audioSource.clip = clip;
		_audioSource.volume = Volume;
		_audioSource.minDistance = MinDistance;
		_audioSource.maxDistance = MaxDistance;
		_audioSource.dopplerLevel = DopplerLevel;
		_audioSource.Play();
		Destroy(_myGameObject, clip.length);
	}
}