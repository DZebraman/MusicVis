using UnityEngine;
using System.Collections;

public class MusicFollow_Follower : MonoBehaviour {

	// Use this for initialization




	void Start () {
	
	}

	protected Vector3 Seek (Vector3 targetPos)
	{
		//find dv, desired velocity
		dv = targetPos - transform.position;		
		dv = dv.normalized; 	//scale by maxSpeed
		dv -= velocity;
		return dv;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
