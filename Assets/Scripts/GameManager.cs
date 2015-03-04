
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	GameObject myGuy; // a seeker
	GameObject targ; // the object of his desire

	//prefabs
	public GameObject TargetPrefab;
	public GameObject GuyPrefab;
	public GameObject ObstaclePrefab;
	public  GameObject[] obstacles;

	// These weights will be exposed in the Inspector window
	public float seekWt = 75.0f;
	public float avoidWt = 10.0f;
	public float avoidDist = 10.0f;
	public float wanderWt = 10.0f;

	public int howMany;

	
	void Start () {
		//make a target
		Vector3 pos = new Vector3(Random.Range(-400, 400), 0f, Random.Range( -400, 400));
		targ =	 (GameObject)GameObject.Instantiate(TargetPrefab, pos, Quaternion.identity);

		//make a Seeker
		pos = new Vector3 (0, 1.0f, 0);
		//myGuy = (GameObject)GameObject.Instantiate(GuyPrefab, pos, Quaternion.identity);
		//myGuy.GetComponent<Seeker>().target = targ;
		
		//make some obstacles
		for (int i=0; i< howMany; i++) 
		{
			pos =  new Vector3(Random.Range(-300, 300), Random.Range(-300, 300), Random.Range(-300, 300));
			Quaternion rot = Quaternion.Euler(0, Random.Range(0, 90), 0);
			GameObject.Instantiate(ObstaclePrefab, pos, rot);
		}
		obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");

		
		//tell the camera to follow myGuy
		//Camera.main.GetComponent<SmoothFollow>().target = myGuy.transform;
		
	}

	bool NearAnObstacle(){
		for (int i=0; i<obstacles.Length; i++) {
			if(Vector3.Distance(targ.transform.position, obstacles[i].transform.position) < 6.0f){
				return true;
			}
		}
		return false;
	}
	
	// move the target when myGuy gets close
	void Update () {
		/*
		if(Vector3.Distance( myGuy.transform.position, targ.transform.position) < 1.75f) 
		{
			do
			{
				targ.transform.position = new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
			}
			while(NearAnObstacle());
		}
		*/
	}
}
