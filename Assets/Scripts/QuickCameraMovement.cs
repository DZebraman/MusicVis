using UnityEngine;
using System.Collections;

public class QuickCameraMovement : MonoBehaviour {

	private GameObject butts;
	public float speed;

	// Use this for initialization
	void Start () {
		butts = (GameObject)Instantiate(Resources.Load("Sphere-Cube"),new Vector3(0, 0, 0), Quaternion.identity);
		butts.renderer.enabled = false;
		Camera.main.GetComponent<SmoothFollow> ().target = butts.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.A))
			butts.transform.position = new Vector3(butts.transform.position.x - (speed * Time.deltaTime),butts.transform.position.y,butts.transform.position.z);
		if(Input.GetKey(KeyCode.W))
			butts.transform.position = new Vector3(butts.transform.position.x,butts.transform.position.y+(speed * Time.deltaTime),butts.transform.position.z);
		if(Input.GetKey(KeyCode.S))
			butts.transform.position = new Vector3(butts.transform.position.x,butts.transform.position.y -(speed * Time.deltaTime),butts.transform.position.z);
		if(Input.GetKey(KeyCode.D))
			butts.transform.position = new Vector3(butts.transform.position.x +(speed * Time.deltaTime),butts.transform.position.y,butts.transform.position.z);
	}
}
