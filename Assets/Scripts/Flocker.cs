using UnityEngine;
using System.Collections;
//including some .NET for dynamic arrays called List in C#
using System.Collections.Generic;


public class Flocker : Vehicle
{
	protected FlockManager flockManager;

	// a unique identification number assigned by the flock manager
	// for the purpose of using the distances array
	protected int index = -1;
	public int Index {
		get { return index; }
		set { index = value; }
	}

	protected Vector3 localCentroid;

	protected  GameObject[] obstacles; 

	protected GameObject mainGO;
	protected GameManager gameManager;
	protected Vector3 localDirection;
	public float dynaFlockDist;
	public int FollowWeight;

	override public void Start ()
	{
		base.Start ();


		obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");
		mainGO = GameObject.Find ("mainGO");
		gameManager = mainGO.GetComponent<GameManager>();

		//get component references
		flockManager = FlockManager.Instance;
	}


	//This function depends on the flockManager updating the direction
	//of the flock every frame. It makes use of the AlignTo function
	//that has been added to the Vehicle class.
	protected Vector3 Alignment ()
	{
		return AlignTo (localDirection);
	}

	
	//Seeks the center of all the flockers within a radius
	protected Vector3 Cohesion ()
	{
		return Seek (localCentroid);
	}


	protected void LocalFlock ()
	{ 
		localCentroid = Vector3.zero;
		//float dot;
		int localCount = 0;

		float distBetween;
		localDirection = new Vector3 (0, 0, 1);


		//Figures out where the center of all the local flockers is
		for(int i = 0; i < flockManager.Flockers.Count; i++)
		{
			distBetween = Vector3.Distance(flockManager.Flockers[i].transform.position, transform.position);

			//Radius Based
			if(distBetween < flockManager.DynaFlockDist)
			{
				localDirection += flockManager.Flockers[i].transform.forward;
				localCount++;
				localCentroid += flockManager.Flockers[i].transform.position;///(distBetween*0.75f);
				Debug.DrawLine (transform.position,flockManager.Flockers[i].transform.position, Color.blue);
			}

		}
		localCentroid /= localCount;
		//Debug.DrawLine (transform.position,localCentroid, Color.red);
	}

	private Vector3 Follow()
	{
		float distBetween;
		float closest = 1000;
		int indexOf = 0;
		//finds the closest leader and follows it
		for (int i = 0; i < flockManager.Leaders.Count; i++) {
			distBetween = flockManager.Distances[this.index, i];
			if(distBetween < closest){
				indexOf = i;
				closest = distBetween;
			}
		}

		Leader temp = flockManager.Leaders [indexOf].GetComponent<Leader> ();
		Debug.DrawLine (transform.position, temp.FollowPoint,Color.green);
		Debug.DrawLine (transform.position,transform.position + transform.forward, Color.red);

		return Seek(temp.FollowPoint);
	}

	protected Vector3 PredatorCheck()
	{
		Vector3 fleeVector = Vector3.zero;
		//Checks if a predator is within a threatening range, Flees from it if it is
		for (int i = 0; i < flockManager.Predators.Count; i++) {
			if(Vector3.Distance(transform.position, flockManager.Predators[i].transform.position) < flockManager.FleePredatorDist)
				fleeVector += Flee (flockManager.Predators[i].transform.position)*500;
		}
		return fleeVector;
	}

	 protected override void CalcSteeringForce ()
	{
		Vector3 force = Vector3.zero;
		LocalFlock();

		//force += transform.forward * 8;

		// handle obstacle avoidance and containment here
		// if I'm stuck (not moving much) wander

		//seek target
		force += flockManager.alignmentWt * Alignment ();
		force += flockManager.cohesionWt * Cohesion ();
		force += flockManager.separationWt * Separation (flockManager.Flockers, flockManager.separationDist, flockManager.Distances);
			
		//if target is not behind me avoid obstacles
			for (int i=0; i<obstacles.Length; i++) {	
				force += flockManager.avoidWt * AvoidObstacle (obstacles [i], gameManager.avoidDist);
			}

		force += Follow () * FollowWeight;

		force += PredatorCheck ();

		//limit force to maxForce and apply
		force = Vector3.ClampMagnitude (force, maxForce);
		//Debug.DrawLine (transform.position,transform.position + force, Color.green);
		ApplyForce(force);
	}
	
}