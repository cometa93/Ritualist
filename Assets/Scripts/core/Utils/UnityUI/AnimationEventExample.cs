using UnityEngine;
using System.Collections;

public class AnimationEventExample : MonoBehaviour
{

	public AudioSource audioSource;

	public void PlayAudioClip(AudioClip audioClip)
	{
		if (audioSource && audioClip)
			audioSource.PlayOneShot(audioClip);
		else
			Debug.Log ("Audio source or clip missing");
	}
}
