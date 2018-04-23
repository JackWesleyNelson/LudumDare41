using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {
	public Transform guiToDisable;
	public Transform guiToEnable;
	private bool paused;

	void Awake(){
		paused = false;
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			paused = !paused;
			if(paused){

				Time.timeScale = 0;
				//Deactivate everything
				guiToDisable.gameObject.SetActive(false);
				//Enable the splash
				guiToEnable.gameObject.SetActive (true);
			}
			else{
				//Disable the splash
				guiToEnable.gameObject.SetActive(false);
				guiToDisable.gameObject.SetActive (true);
				//reset timescale
				Time.timeScale = 1;
			}
		}
	}

	public void RestartScene(){
		Time.timeScale = 1;
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	public void ExitGame(){
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit ();	
		#endif
	}


}
