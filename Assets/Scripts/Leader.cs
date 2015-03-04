using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Leader : Flocker{

	public Vector3 FollowPoint;

	// Use this for initialization
	public override void Start () {
		base.Start ();
	}

	// Update is called once per frame
	void LateUpdate () {
		FollowPoint = Vector3.zero;
		FollowPoint = transform.position - (transform.forward.normalized * flockManager.FollowDistance);
		Debug.DrawLine (transform.position, FollowPoint, Color.magenta);
		base.LateUpdate();
	}

	protected void LocalFlock (int flockDistance)
	{ 
		localCentroid = Vector3.zero;
		//float dot;
		int localCount = 0;
		
		localDirection = new Vector3 (0, 0, 1);

		//Flocks with other leaders just like the flockers
		for(int i = 0; i < flockManager.Leaders.Count; i++)
		{
			float distBetween = Vector3.Distance(flockManager.Leaders[i].transform.position, transform.position);

			if(distBetween < flockDistance)
			{
				localDirection += flockManager.Leaders[i].transform.forward;
				localCount++;
				localCentroid += flockManager.Leaders[i].transform.position;///(distBetween*0.75f);
				//Debug.DrawLine (transform.position,flockManager.Leaders[i].transform.position, Color.gray);
			}
			
		}
		localCentroid /= localCount;
		//Debug.DrawLine (transform.position,localCentroid, Color.red);
	}

	protected Vector3 Separation (List<GameObject> flockers, float dist, float[,] distances)
	{

		Vector3 sepForce = Vector3.zero;
		for(int i = 0; i < flockers.Count; i++)
		{
			float distBetween = Vector3.Distance(flockers[i].transform.position, transform.position);
			if(distBetween < dist)
			{
				sepForce += Flee(flockers[i].transform.position);
			}
		}

		return sepForce;
	}

	protected override void CalcSteeringForce ()
	{
		Vector3 force = Vector3.zero;
		LocalFlock(flockManager.leaderFlockDist);
		
			//seek target
			force += flockManager.alignmentWt * Alignment ();
			force += flockManager.cohesionWt * Cohesion ();
			force += flockManager.leaderSeparationWt * Separation (flockManager.Leaders, flockManager.leaderSeparationDist, flockManager.LeaderDistances);
			
			//if target is not behind me avoid obstacles
			for (int i=0; i<obstacles.Length; i++) {	
				force += flockManager.avoidWt * AvoidObstacle (obstacles [i], gameManager.avoidDist);
			}

		
		float toCentroid = Vector3.Distance (flockManager.Centroid, transform.position);
		force += Seek (flockManager.Centroid * Mathf.Pow (toCentroid, 2) / flockManager.TotalCohesion);

		force += PredatorCheck ();

		//limit force to maxForce and apply
		force = Vector3.ClampMagnitude (force, maxForce);
		//Debug.DrawLine (transform.position,transform.position + force, Color.green);
		ApplyForce(force);
	}
}
