using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RhythmManager : MonoBehaviour {
	public Transform[] columns;
	public Transform[] notes;
	public Transform chances;
	public Transform chance;
	public Transform chanceUsed;
	public Transform blank;
	public AudioClip[] oneShotPlayed;
	public AudioClip oneShotMissed, backgroundMusic;
	//Keycodes for adding and removing
	private KeyCode[] arrows;
	private KeyCode[] rowNums;
	private string[] tags;
	private AudioSource audioSource;
	private float elapsedTime;
	private bool movedDuringbackgroundMusic;
	private List<int> columnIndexes;
	private StealthManager stealthManager;
	private int chancesMax;

	public int chancesLeft;

	void Awake(){
		chancesMax = chancesLeft;
		for(int i = 0 ; i < chancesLeft; i++){
			Instantiate (chance, chances);
		}

		stealthManager = gameObject.GetComponent<StealthManager> ();
		elapsedTime = 0.0f;
		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = backgroundMusic;
		audioSource.loop = true;
		audioSource.Play ();
		arrows = new KeyCode[] {
			KeyCode.LeftArrow,
			KeyCode.UpArrow,
			KeyCode.DownArrow,
			KeyCode.RightArrow
		};
		rowNums = new KeyCode[] {
			KeyCode.Alpha1,
			KeyCode.Alpha2,
			KeyCode.Alpha3,
			KeyCode.Alpha4
		};
		tags = new string[] {
			"Note_Green",
			"Note_Magenta",
			"Note_Blue",
			"Note_Orange"
		};
		columnIndexes = new List<int> () {
			0,
			1,
			2,
			3
		};

		foreach(Transform column in columns){
			for(int i = 0; i < 10; i++){
				Instantiate (blank, column);
			}
		}
	}

	void Update(){
		if(chancesLeft <= 0){
			SceneManager.LoadScene ("GameOver");
		}

		if(Time.timeScale == 0){
			audioSource.Pause ();
		}
		else{
			if(audioSource.isPlaying == false){
				audioSource.UnPause ();
			}
			//The time has surpassed it's limit
			if(elapsedTime > backgroundMusic.length){
				//reset time
				elapsedTime = 0.0f;
				elapsedTime += Time.deltaTime;
				//the user moved, so reset the bool and carry on.
				if(movedDuringbackgroundMusic){
					movedDuringbackgroundMusic = false;
				}//The user hasn't moved and the time to do so is up, evaluate and drop new random notes.
				else{
					while(true){
						EvaluateBottom ();
						//Add two random values
						List<int> randomColumnIndexes = columnIndexes.ToList();
						int index = randomColumnIndexes [Random.Range (0, randomColumnIndexes.Count)];
						AddNoteToColumnStart (index);
						randomColumnIndexes.Remove (index);
						index = randomColumnIndexes [Random.Range (0, randomColumnIndexes.Count)];
						AddNoteToColumnStart (index);
						//Add the blank spaces that weren't chosen
						randomColumnIndexes.Remove (index);
						foreach(int i in randomColumnIndexes){
							AddBlankToColumnStart (i);
						}
						break;
					}

				}
			}//The user still has time, check their actions.
			else{
				elapsedTime += Time.deltaTime;
				//Remove and play sound for any played notes.
				foreach(int i in columnIndexes){
					if(RemoveNoteIfPlayed (i)){
						audioSource.PlayOneShot (oneShotPlayed [i]);
					}
				}
				//Check if we are walking in any direction
				foreach(int i in columnIndexes){
					if(AddNoteWhenWalking(i)){
						//Get the remaining column choices for a random note to appear.
						List<int> randomColumnChoices = columnIndexes
							.Where (x => (x != i))
							.ToList ();
						//Select the column for the random note to appear.
						int randomColumn = randomColumnChoices[Random.Range (0, randomColumnChoices.Count)];
						//add the random note.
						AddNoteToColumnStart (randomColumn);
						//remove the random column choice from the temp list
						randomColumnChoices.Remove (randomColumn);
						//for the remaining items in the list add a blank note
						foreach(int blankColumnIndex in randomColumnChoices){
							AddBlankToColumnStart (blankColumnIndex);
						}
						//break out so we don't move another direction.
						break;
					}
				}
			}
		}
	}
	//returns true if the user walked, therefore adding a note.
	private bool AddNoteWhenWalking (int columnIndex){
		if(Input.GetKeyDown(arrows[columnIndex])){
			//Let the time loop know that we've moved.
			movedDuringbackgroundMusic = true;
			//Check if any notes remain after the user had a chance to play them.
			EvaluateBottom ();
			//Add the new note
			AddNoteToColumnStart (columnIndex);
			//Let the update loop know the user walked.
			return true;
		}
		return false;
	}
	//Returns true if the user played a note
	private bool RemoveNoteIfPlayed(int columnIndex){
		//Check if the columns key was pressed
		if(Input.GetKeyDown(rowNums[columnIndex])){
			//Check if there is a note there
			if(columns[columnIndex].GetChild(columns[columnIndex].childCount-1).gameObject.CompareTag(tags[columnIndex])){
				//destroy the note
				RemoveNoteFromColumn(columnIndex);
				//add a blank space at the end
				Instantiate (blank, columns [columnIndex]).SetAsLastSibling();
				return true;
			}
			else{
				LoseChance();
			}
		}
		return false;
	}

	private void AddNoteToColumnStart(int columnIndex){
		Instantiate (notes [columnIndex], columns [columnIndex]).SetAsFirstSibling();
		RemoveNoteFromColumn (columnIndex);
	}
	private void AddBlankToColumnStart(int columnIndex){
		Instantiate (blank, columns [columnIndex]).SetAsFirstSibling();
		RemoveNoteFromColumn (columnIndex);
	}
	private void RemoveNoteFromColumn(int columnIndex){
		GameObject.Destroy (columns [columnIndex].GetChild (columns[columnIndex].childCount-1).gameObject);
	}

	//Check if the user is pushing notes off the column
	private void EvaluateBottom (){
		foreach(Transform column in columns){
			if(!column.GetChild(column.childCount-1).gameObject.CompareTag("Note_Blank")){
				LoseChance();
				break;
			}
		}
	}

	private void LoseChance(){
		audioSource.PlayOneShot (oneShotMissed);
		chancesLeft--;
		foreach(Transform child in chances){
			Destroy (child.gameObject);
		}
		for(int i = 0 ; i < chancesLeft; i++){
			Instantiate (chance, chances);
		}
		for(int i = chancesLeft; i < chancesMax; i++){
			Instantiate (chanceUsed, chances);
		}
	}

}
