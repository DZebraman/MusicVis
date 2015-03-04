using UnityEngine;
using System.Collections;

public class Predator : Vehicle {

	protected FlockManager flockManager;

	protected  GameObject[] obstacles; 
	
	protected GameObject mainGO;
	protected GameManager gameManager;

	//Max is declared in the inspector
	public int max;

	private int target;

	public int Index;
	float hunger = 0;

	// Use this for initialization
	void Start () {
		hunger = Random.Range (max/2, max);
		base.Start ();
		//get component references
		obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");
		mainGO = GameObject.Find ("mainGO");
		gameManager = mainGO.GetComponent<GameManager>();
		flockManager = FlockManager.Instance;
	}

	//Picks a random leader as a target
	void FindTarget()
	{ 
		target = Random.Range (0, flockManager.Leaders.Count - 1);
	}

	protected override void CalcSteeringForce ()
	{
		Vector3 force = Vector3.zero;

		for (int i=0; i<obstacles.Length; i++) 
			force += flockManager.avoidWt * AvoidObstacle (obstacles [i], gameManager.avoidDist);

		//if it's hungry, attack otherwise flee
		if (hunger > max/2)
			force += Seek (flockManager.Leaders [target].transform.position) * (2*hunger);
		else
			force += Flee (flockManager.Leaders [target].transform.position) * 1/hunger;
		
		//limit force to maxForce and apply
		force = Vector3.ClampMagnitude (force, maxForce);
		//Debug.DrawLine (transform.position,transform.position + force, Color.green);
		ApplyForce(force);
	}

	// Update is called once per frame
	void Update () {
		hunger += Time.deltaTime;
		if (hunger > max)
		{
			hunger = 0.0001f;
			FindTarget();
		}
	}
}
