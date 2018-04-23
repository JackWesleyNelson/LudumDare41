using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.SceneManagement;

public class StealthManager : MonoBehaviour {
	public Transform player, key, keyHole;
	public Transform guards;
	public int stepSize;
	public float timeBetweenGuardMovement;
	public string victoryScene;

	public Tilemap floorTilemap, wallTilemap;
	public Tile[] floors;
	public int width, height;
	private KeyCode[] arrows;
	private Vector3[] movement;
	private float time;
	private bool hasKey;

	private List<Vector3Int> walkableSpaces;
	private Dictionary<Vector3Int, List<Vector3>> validMovements;
	private Dictionary<Transform, Queue<Vector3>> guardPaths;


	void Awake(){
		hasKey = false;

		time = 0.0f;

		arrows = new KeyCode[] {
			KeyCode.LeftArrow,
			KeyCode.UpArrow,
			KeyCode.DownArrow,
			KeyCode.RightArrow
		};
		movement = new Vector3[] {
			new Vector3(-1*stepSize, 0, 0),
			new Vector3(0, stepSize, 0),
			new Vector3(0, -1*stepSize, 0),
			new Vector3(stepSize, 0, 0)
		};

		validMovements = new Dictionary<Vector3Int, List<Vector3>> ();

		walkableSpaces = new List<Vector3Int> ();
		for(int x = 0; x < width; x++){
			for(int y = -1; y >= height*-1; y--){
				floorTilemap.SetTile (new Vector3Int (x, y, 0), floors [Random.Range (0, floors.Length)]);
				if(wallTilemap.GetTile(new Vector3Int(x, y, 0)) == null){
					Vector3Int position = new Vector3Int (x, y, 0);
					walkableSpaces.Add (position);
					validMovements.Add (position, GetNeighbors (position));
				}
			}
		}

		guardPaths = new Dictionary<Transform, Queue<Vector3>> ();
		foreach(Transform guard in guards){
			guardPaths.Add (guard, new Queue<Vector3> ());
		}

	}


	void Update(){
		if(Time.timeScale != 0){
			time += Time.deltaTime;
			//Move the player
			for(int i = 0; i < arrows.Length; i++){
				if(Input.GetKeyDown(arrows[i])){
					Vector3 newPos = movement[i] + player.localPosition;
					if (validMovement (newPos)) {
						player.localPosition = newPos;
						foreach (Transform guard in guards) {
							if (player.localPosition == guard.localPosition) {
								SceneManager.LoadScene ("GameOver");
							}	
						}
						if (key != null && player.localPosition == key.localPosition) {
							GameObject.Destroy (key.gameObject);
							hasKey = true;
						}
						if (player.localPosition == keyHole.localPosition && hasKey) {
							SceneManager.LoadScene (victoryScene);
						}
						break;
					}
				}	
			}
			//Move the guards
			if(time > timeBetweenGuardMovement){
				foreach(Transform guard in guards){
					MoveGuard (guard);
				}
				time = 0.0f;
			}
		}
	}

	private bool validMovement(Vector3 newPos){
		if(wallTilemap.GetTile(Vector3ToVector3IntWithPlayerOffset(newPos)) == null){
			return true;
		}
		return false;
	}


	private void MoveGuard(Transform guard){
/*		//If they have a path follow it.
		if(guardPaths[guard].Count > 0){
			guard.localPosition += guardPaths[guard].Dequeue ();
		}//No path, generate one then move.
		else{
			Vector3 destination = walkableSpaces [Random.Range (0, walkableSpaces.Count - 1)];
			while(destination == guard.localPosition){
				destination = walkableSpaces [Random.Range (0, walkableSpaces.Count - 1)];
			}
			guardPaths [guard] = getPath (destination, guard.localPosition);
			guard.localPosition += guardPaths [guard].Dequeue ();
		}*/
		List<Vector3> neighbors = GetNeighbors (guard.localPosition);
		guard.localPosition += neighbors[Random.Range(0, neighbors.Count-1)];
		if(player.localPosition == guard.localPosition){
			SceneManager.LoadScene ("GameOver");
		}	
	}
/*
	private Queue<Vector3> getPath(Vector3 destination, Vector3 currentLocation){
		//List<Queue<Vector3>> paths = new List<Queue<Vector3>> ();
		Queue<Vector3> path = new Queue<Vector3> ();

		while(destination != currentLocation){
			foreach(Vector3 direction in validMovements[Vector3ToVector3Int(currentLocation)]){
				currentLocation += direction;
				path.Enqueue (direction);
			}
		}
		return path;
	}*/


	//Might need to use offset version if guards go through the tilemap areas.
	private List<Vector3> GetNeighbors(Vector3 currentSpace){
		List<Vector3> neighbors = new List<Vector3> ();
		foreach(Vector3 v in movement){
			try{
				
				if(wallTilemap.GetTile(Vector3ToVector3IntWithPlayerOffset(currentSpace) + Vector3ToVector3Int(v)) == null){
					neighbors.Add(v);
				}	
			}
			catch(System.NullReferenceException ){
			}
		}
		return neighbors;
	}

	private Vector3 Vector3IntToVector3(Vector3Int v){
		return new Vector3 ((float)v.x, (float)v.y, (float)v.z);
	}

	private Vector3Int Vector3ToVector3Int(Vector3 v){
		return new Vector3Int ((int)v.x, (int)v.y, (int)v.z);
	}

	private Vector3Int Vector3ToVector3IntWithPlayerOffset(Vector3 v){
		return new Vector3Int ((int)v.x, (int)v.y -1, (int)v.z);
	}


}
