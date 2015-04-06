using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour {

	public CharacterController leaderController;

	private Vector3 v;
	private Vector3 a;

	public float speed;

	private List<GameObject> bullets = new List<GameObject>();
	private GameObject cursor;

	private int nextBullet;

	private Vector3 mousePos;

	public int numBullets;
	private GameObject[] enemies;

	// Use this for initialization
	void Start () {
		leaderController = GetComponent<CharacterController> ();
		cursor = (GameObject)Instantiate(Resources.Load("Sphere-Cube"), new Vector3(0, 0, -6), Quaternion.identity);
		cursor.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);

		for (int i = 0; i < numBullets; i++) {
			bullets.Add((GameObject)Instantiate (Resources.Load("BulletPrefab"), new Vector3(i,0,-100), Quaternion.identity));
			bullets[i].renderer.material.color = Color.blue;
		}

		nextBullet = 0;

		enemies = GameObject.FindGameObjectsWithTag ("Enemy");

	}

	void GetMousePos()
	{
		Vector3 temp;

		mousePos = Input.mousePosition;
		//moves the objects coords to screenspace
		temp = Camera.main.WorldToScreenPoint (this.transform.position);

		//fixes the offset
		temp.x -= Screen.width/2;
		temp.y -= Screen.height / 2;

		temp = mousePos - temp;
		mousePos = Camera.main.ScreenToWorldPoint (temp);
		mousePos.z = -6;

		//inverts x and y
		mousePos.x *= -1;
		mousePos.y *= -1;


		mousePos += this.transform.position;
		mousePos = mousePos.normalized * 5;
		mousePos = this.transform.position + mousePos;
		//Debug.DrawLine (this.transform.position, this.transform.position + mousePos);
	}

	void CalcMovement()
	{
		a = Vector3.zero;
		v *= 0.95f;
		if (Input.GetKey (KeyCode.A))
			a.x -= speed/2 * Time.deltaTime;
		if(Input.GetKey(KeyCode.W))
			a.y += speed/2 * Time.deltaTime;
		if(Input.GetKey(KeyCode.S))
			a.y -= speed/2 * Time.deltaTime;
		if(Input.GetKey(KeyCode.D))
			a.x += speed/2 * Time.deltaTime;
		
		v += a;
		v = v.normalized * Mathf.Clamp(v.magnitude, 0, speed);

		leaderController.Move (v);
	}

	void Fire(){
		Bullet thisBullet = (Bullet)bullets [nextBullet].GetComponent ("Bullet");
		//bullets [nextBullet].transform.position = this.transform.position;

		thisBullet.Fire (mousePos - this.transform.position);
		if (nextBullet < bullets.Count -1)
			nextBullet++;
		else
			nextBullet = 0;
	}

	void calcDistances( )
	{
		float smallest = 1000;
		float temp = 0;
		int index = 0;
		for (int i = 0; i < bullets.Count; i++) {
			smallest = 1000;
			temp = 0;
			index = 0;
			Bullet thisBullet = (Bullet)bullets [i].GetComponent ("Bullet");
			for(int k = 0; k < enemies.Length; k++){
				temp = Vector3.Distance(bullets[i].transform.position,enemies[k].transform.position);
				if(temp < smallest){
					smallest = temp;
					index = k;
				}
			}
			thisBullet.Target = index;
		}
	}

	// Update is called once per frame
	void Update () {
		CalcMovement ();
		GetMousePos ();
		calcDistances ();
		leaderController.Move (v);
		cursor.transform.position = mousePos;

		if (Input.GetMouseButton (0) && Time.frameCount % 7 == 0) {
			Fire();
		}
	}
}
