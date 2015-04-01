using UnityEngine;
using System.Collections;

public class QuickCameraMovement : MonoBehaviour {

	private GameObject butts;
	public GameObject toFollow;
	public float speed;

	private Vector3 v;
	private Vector3 a;

	// Use this for initialization
	void Start () {
		butts = (GameObject)Instantiate(Resources.Load("Sphere-Cube"),new Vector3(0, 0, 0), Quaternion.identity);
		butts.renderer.enabled = false;
		toFollow.GetComponent<SmoothFollow> ().target = butts.transform;
	}
	
	// Update is called once per frame
	void Update () {
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
		butts.transform.position += v;
	}
}
