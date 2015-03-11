using UnityEngine;
using System.Collections;

public class MouseFollowMusic : MonoBehaviour {
	
	//Fireball Variables
	public int maxBalls; //Equal to the number of fireballs spawned in Start()
	public float speed; //This float will be multiplied by a Vector3 to deterimine launch speed and angle
	//private int[] nextFireball; //Which index of fireBallsArray will get launched next?
	private GameObject[] fireballsArray; //A 1D array of fireballs.
	private Rigidbody[] rigidbodyArray;  //A 1D array of fireballs' rigidbodies.
	//that sets the spawn point for launched fireballs
	
	
	//Rainbow Variables (Just copy paste all this stuff, it's a bit too tricky to explain right now)
	public float[] rgb;
	public  int rainbowIndex;
	
	public GameObject thisObj;
	public AudioListener listener;
	
	public float freqStrength;
	
	public float UpperBound;
	public float LowerBound;
	
	public float dispAmount;
	public float scaleAmount;
	
	private float dispActual;
	
	private int conversion;
	public float lerpSpeed;
	
	public float colorIntensity;
	
	public float noiseThreshold;
	
	public AudioSource aud;


	protected Vector3 dv;
	protected Vector3 velocity;

	Vector3 camTarget;
	
	// Use this for initialization
	void Start () {
		
		dispActual = 0;
		//nextFireball = new int[2];
		
		fireballsArray = new GameObject[maxBalls]; //Sets how big our 1D array of GameObjects is
		rigidbodyArray = new Rigidbody[maxBalls]; //We're putting these components in their own array because using GetComponent at runtime is relatively slow
		for (int i = 0; i < maxBalls; i++){ //Iterates through every fireball reference, instantiating a prefab and assigning it a GameObject reference in our 1D array 
			fireballsArray[i] = (GameObject)Instantiate(Resources.Load("fireballPrefab"), new Vector3(i*3, 0, 0), Quaternion.identity); 
			fireballsArray[i].name = "Fireball " + i; //Names the prefab (visible in hierarchy window) 
			rigidbodyArray[i] = fireballsArray[i].GetComponent<Rigidbody>(); //Assigns our rigidbody REFERENCE IN THE ARRAY to look up this specific fireball's rigidbody
		}
		


		//InvokeRepeating("ScaleToSound", 1.0f, .05f);
		aud = GetComponent<AudioSource>();
	}

	float NormalizeVolume(float[] spectrum)
	{
		float avg = 0;
		for (int i = 0; i < spectrum.Length; i++) {
			avg += spectrum[i];
		}
		avg /= spectrum.Length;

		return avg;
	}

	void ScaleToSound()
	{
		float[] spectrum = aud.GetSpectrumData(4096, 0, FFTWindow.Blackman);
		conversion = spectrum.Length/(maxBalls);
		float temp = 0;
		float avg = NormalizeVolume (spectrum);

		for (int i = 0; i < maxBalls; i++) {

					temp = 0;
					temp += spectrum[i];
					//temp /= conversion;
					temp /= avg;		

					temp *= scaleAmount * i;
					temp /= 8;

					if(temp <= 0.5f)
						temp = 0.5f;
					if(temp >= 8f)
						temp = 8f;
					
				fireballsArray[i].transform.localScale = new Vector3(0,0,0) + Vector3.Lerp(fireballsArray[i].transform.localScale, new Vector3(temp,temp,temp), Time.deltaTime * lerpSpeed*100);
				fireballsArray[i].renderer.material.color = Color.Lerp(Color.black, new Color((1-Mathf.Pow(fireballsArray[i].transform.localScale.y,colorIntensity) * 1.2f), 0, 0), lerpSpeed);

			}		
		}

	// Update is called once per frame
	void Update () {
	
		Camera.main.GetComponent<SmoothFollow> ().target = fireballsArray[0].transform;

		Vector3 test = new Vector3 (Input.mousePosition.x , Input.mousePosition.y, 0);


		for (int i = 0; i < fireballsArray.Length; i++) {
			if( i < 1)	
				fireballsArray[i].transform.position = Vector3.Lerp (fireballsArray [i].transform.position, test, lerpSpeed/500 * Time.deltaTime);
			else
				fireballsArray[i].transform.position = Vector3.Lerp(fireballsArray[i].transform.position, fireballsArray[i-1].transform.position, lerpSpeed * Time.deltaTime * 1.3f);
		}

		ScaleToSound ();
	}
}
