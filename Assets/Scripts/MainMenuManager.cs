using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour {

	public AudioClip mainMenuMusic;
	private AudioSource audioSource;

	void Awake(){
		audioSource = gameObject.AddComponent<AudioSource> ();
		audioSource.clip = mainMenuMusic;
		audioSource.loop = true;
		audioSource.Play();
	}

	public void StartGame(){
		SceneManager.LoadScene ("Main");
	}
	public void LoadHelp(){
		SceneManager.LoadScene ("Help");
	}

	public void ExitGame(){
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit ();	
		#endif
	}


}
