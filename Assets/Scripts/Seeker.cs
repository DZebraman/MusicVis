using UnityEngine;
using System.Collections;

public class Seeker : Vehicle {
	public GameObject target;
	//reference to an array of obstacle
	private  GameObject[] obstacles; 

	
	private GameObject mainGO;
	private GameManager gameManager;


	// Call Inerited Start and then do our own
	override public void Start () {
		base.Start();
		obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");
		mainGO = GameObject.Find ("mainGO");
		gameManager = mainGO.GetComponent<GameManager>();
	}
	
	// All vehicles need to override CalcSteeringForce
	protected override void CalcSteeringForce(){
		Vector3 force = Vector3.zero;

		// if I'm stuck (not moving much) wander
		if (characterController.velocity.magnitude < 1.0f) {
			force += Wander() * gameManager.wanderWt;
		} 
		else {
			//seek target
			force += gameManager.seekWt * Seek (target.transform.position);

			//if target is not behind me avoid obstacles
			Vector3 toTarget = target.transform.position - transform.position;
			if (Vector3.Dot(toTarget, transform.forward) > 0){
				for (int i=0; i<obstacles.Length; i++) {	
				   force += gameManager.avoidWt * AvoidObstacle (obstacles [i], gameManager.avoidDist);
				}
			}
		}

		//limit force to maxForce and apply
		force = Vector3.ClampMagnitude (force, maxForce);
		ApplyForce(force);

		//show force as a blue line pushing the guy like a jet stream
		Debug.DrawLine(transform.position, transform.position - force,Color.blue);
		//red line to the target which may be out of sight
		Debug.DrawLine (transform.position, target.transform.position,Color.red);

	}
}
