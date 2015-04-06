using UnityEngine;
using System.Collections;

public class QuickMove : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position -= new Vector3 (-0.125f, 0, 0);
	}
}
