using UnityEngine;
using System.Collections;
//including some .NET for dynamic arrays called List in C#
using System.Collections.Generic;


public class FlockManager : MonoBehaviour
{
	// weight parameters are set in editor and used by all flockers 
	// if they are initialized here, the editor will override settings	 
	// weights used to arbitrate btweeen concurrent steering forces 
	public float alignmentWt;
	public float separationWt;
	public float leaderSeparationWt;
	public float cohesionWt;
	public float avoidWt;
	public float inBoundsWt;


	public int FollowDistance;

	public int ContainAmount;
	public int TotalCohesion;
	public int Spread;
	public int FOV;
	public int DynaFlockDist;
	public int leaderFlockDist;
	public int SightDist;
	public int FleePredatorDist;


	// these distances modify the respective steering behaviors
	public float avoidDist;
	public float separationDist;
	public float leaderSeparationDist;
	

	// set in editor to promote reusability.
	public int numberOfFlockers;
	public int numberOfLeaders;
	public int NumPredators;

	public Object flockerPrefab;
	public Object LeaderPrefab;
	public Object PredatorPrefab;
	public Object obstaclePrefab;
	
	//values used by all flockers that are recalculated on update
	private Vector3 direction;
	private Vector3 centroid;
	
	//accessors
	private static FlockManager instance;
	public static FlockManager Instance { get { return instance; } }

	public Vector3 FlockDirection {
		get { return direction; }
	}
	
	public Vector3 Centroid { get { return centroid; } }
	public GameObject centroidContainer;
	
		
	// list of flockers with accessor
	private List<GameObject> flockers = new List<GameObject>();
	public List<GameObject> Flockers {get{return flockers;}}

	private List<GameObject> leaders = new List<GameObject>();
	public List<GameObject> Leaders {get{return leaders;}}

	private List<GameObject> predators = new List<GameObject>();
	public List<GameObject> Predators {get{return predators;}}

	// array of obstacles with accessor
	private  GameObject[] obstacles;
	public GameObject[] Obstacles {get{return obstacles;}}
	
	// this is a 2-dimensional array for distances between flockers
	// it is recalculated each frame on update
	private float[,] distances;
	public float[,] Distances
	{
		get{ return distances;}
	}

	private float[,] leaderDistances;
	public float[,] LeaderDistances
	{
		get{ return leaderDistances;}
	}

		
		//construct our 2d array based on the value set in
	public void Start ()
	{
		//FOV = 0;
		//DynaFlockDist = 0;
		instance = this;
		//construct our 2d array based on the value set in the editor
		distances = new float[numberOfFlockers, numberOfFlockers];
		leaderDistances = new float[numberOfLeaders, numberOfLeaders];
		//reference to Vehicle script component for each flocker
		Flocker flocker; // reference to flocker scripts
		Leader leader;
	
		obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");


		for (int i = 0; i < numberOfLeaders; i++) {
			//Instantiate a flocker prefab, catch the reference, cast it to a GameObject
			//and add it to our list all in one line.
			Vector3 pos = new Vector3(Random.Range(-Spread,Spread), Random.Range(-Spread,Spread), Random.Range(-Spread,Spread));

			leaders.Add ((GameObject)Instantiate (LeaderPrefab, pos, Quaternion.identity));
			//grab a component reference
			leader = leaders [i].GetComponent<Leader> ();
			//set values in the Vehicle script
			leader.Index = i;
		}

		for (int i = 0; i < numberOfFlockers; i++) {
			//Instantiate a flocker prefab, catch the reference, cast it to a GameObject
			//and add it to our list all in one line.
			Vector3 pos = new Vector3(Random.Range(-Spread,Spread), Random.Range(-Spread,Spread), Random.Range(-Spread,Spread));

			flockers.Add ((GameObject)Instantiate (flockerPrefab, pos, Quaternion.identity));
			//grab a component reference
			flocker = flockers [i].GetComponent<Flocker> ();
			//set values in the Vehicle script
			flocker.Index = i;
		}

		//Creates predators in the same manner as flockers
		for (int i = 0; i < NumPredators; i++) {
			Vector3 pos = new Vector3(Random.Range(-Spread* 10,Spread*10), Random.Range(-Spread*10,Spread*10), Random.Range(-Spread*10,Spread*10));
			
			predators.Add ((GameObject)Instantiate (PredatorPrefab, pos, Quaternion.identity));
			//grab a component reference
			Predator predator= predators [i].GetComponent<Predator> ();
			//set values in the Vehicle script
			predator.Index = i;
		}

		centroidContainer.transform.position = new Vector3 (320, 30, 100);
		
	}
	public void Update( )
	{
		calcCentroid( );//find average position of each flocker 
		//calcFlockDirection( );//find average "forward" for each flocker

		//position & orient the centoid container for the SmoothFollow camera script
		centroidContainer.transform.position = centroid;
		centroidContainer.transform.forward = direction;

		// This function which populates the 2-dimensional distance array, lets us 
		// calculate distance between each pair of flockers exactly once per frame
		calcDistances( );
	}

	public void LateUpdate()
	{
		centroidContainer.transform.position = centroid;
		Camera.main.GetComponent<SmoothFollow> ().target = centroidContainer.transform;
	}
	
	
	void calcDistances( )
	{
		float dist;
		for(int i = 0 ; i < numberOfFlockers; i++)
		{
			for( int j = i+1; j < numberOfFlockers; j++)
			{
				dist = Vector3.Distance(flockers[i].transform.position, flockers[j].transform.position);
				distances[i, j] = dist;
				distances[j, i] = dist;
			}
		}
	}

	void calcLeadDistances( )
	{
		float dist;
		for(int i = 0 ; i < numberOfLeaders; i++)
		{
			for( int j = i+1; j < numberOfLeaders; j++)
			{
				dist = Vector3.Distance(leaders[i].transform.position, leaders[j].transform.position);
				leaderDistances[i, j] = dist;
				leaderDistances[j, i] = dist;
			}
		}
	}
	
	public float getDistance(int i, int j)
	{
		return distances[i, j];
	}
		
	private void calcCentroid ()
	{
		for(int i = 0; i < flockers.Count; i++)
		{
			centroid += flockers[i].transform.position;
		}
		centroid /= flockers.Count;
	}
	
//	private void calcFlockDirection ()
//	{
//		//*******************************************************
//		// calculate the average heading of the flock
//		// use transform.forward - you need to write this!
//		//*******************************************************
//
//		direction = new Vector3 (0, 0, 1); //fix this!
//
//		for(int i = 0; i < flockers.Count; i++)
//		{
//			direction += flockers[i].transform.forward;
//		}
//	}
	
}