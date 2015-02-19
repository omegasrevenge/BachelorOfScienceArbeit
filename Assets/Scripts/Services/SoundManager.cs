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

		Properties MyProperties = Properties.Singleton;

		if (MyProperties == null)
			MyProperties = GameObject.FindGameObjectWithTag("GameController").GetComponent<Properties>();

		foreach (string AudioName in MyProperties.SoundFileNames) 
				MyAudioClips.Add (Resources.Load<AudioClip> (Properties.SoundsFolderName + "/" + AudioName));
	}

	public static AudioClip GetClip(int index)
	{
		return SoundManager.Singleton.MyAudioClips [index];
	}

	public static void PlayClipAt(AudioClip clip, Vector3 pos, float volume, float minDistance, float maxDistance, float dopplerLevel = 0f)
	{
		GameObject MyGameObject = new GameObject("TempAudio");
		MyGameObject.transform.position = pos;
		AudioSource AudioSrc = MyGameObject.AddComponent<AudioSource>();
		AudioSrc.clip = clip;
		AudioSrc.volume = volume;
		AudioSrc.minDistance = minDistance;
		AudioSrc.maxDistance = maxDistance;
		AudioSrc.dopplerLevel = dopplerLevel;
		AudioSrc.Play();
		Destroy(MyGameObject, clip.length);
	}
}