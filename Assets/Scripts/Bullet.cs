using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	GameObject[] followerArray;
	public int numFollowers;
	public float lerpSpeed;
	private bool active;

	private Vector3 v;
	private Vector3 dv;
	private Vector3 a;

	public float speed;
	public float seekWgt;

	//The index of the enemy to seek
	public int Target;

	private GameObject[] enemies;

	void Start () {
		followerArray = new GameObject[numFollowers];
		active = false;
		for(int i = 0; i < followerArray.Length; i++){
			followerArray[i] = (GameObject)Instantiate(Resources.Load("Sphere-Cube"),new Vector3(0,0,-100),Quaternion.identity);
			followerArray[i].renderer.enabled = false;
			followerArray[i].renderer.material.color = Color.red;
			followerArray[i].transform.localScale = new Vector3(0.25f,0.25f,0.25f);
		}

		v = Vector3.zero;
		a = Vector3.zero;
		dv = Vector3.zero;
		enemies = GameObject.FindGameObjectsWithTag ("Enemy");
	}

	void CalcMovement()
	{
		a = Vector3.zero;
		Vector3 temp = Vector3.zero;
		if (active) {
			temp =  enemies[Target].transform.position - transform.position;
			temp.Normalize();
			temp *= speed;
			a = temp - v;
		}

		a *= seekWgt / Vector3.Distance(enemies[0].transform.position,transform.position);

		v += a;
		v = v.normalized * Mathf.Clamp(v.magnitude, 0, speed);

		this.transform.position += v;
	}
	

	void TailManage()
	{
		followerArray [0].transform.position = Vector3.Lerp (followerArray [0].transform.position, this.transform.position, Time.deltaTime * lerpSpeed);
		for(int i = 1; i < followerArray.Length; i++){
			followerArray[i].transform.position = Vector3.Lerp(followerArray[i].transform.position, followerArray[i-1].transform.position, Time.deltaTime * lerpSpeed);
		}
	}

	public void Fire(Vector3 startingV)
	{
		startingV.z = 0;

		active = true;
		for(int i = 0; i < followerArray.Length; i++){
			followerArray[i].renderer.enabled = true;
		}


		GameObject leader = GameObject.Find ("Leader 0");

		this.transform.position = leader.transform.position;

		for(int i = 0; i < followerArray.Length; i++){
			followerArray[i].transform.position = leader.transform.position;
		}


		Debug.Log ("Fire");

		v = startingV;
	}

	// Update is called once per frame
	void Update () {
		CalcMovement ();
		TailManage ();
	}
}
