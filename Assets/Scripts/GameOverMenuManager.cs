using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenuManager : MonoBehaviour {

	public AudioClip mainMenuMusic;
	private AudioSource audioSource;

	void Awake(){
		audioSource = gameObject.AddComponent<AudioSource> ();
		audioSource.clip = mainMenuMusic;
		audioSource.loop = true;
		audioSource.Play();
	}

	public void LoadMainMenu(){
		SceneManager.LoadScene ("Menu");
	}

	public void ExitGame(){
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit ();	
		#endif
	}

}
