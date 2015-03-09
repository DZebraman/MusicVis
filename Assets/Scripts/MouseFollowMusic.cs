using UnityEngine;
using System.Collections;

public class MouseFollowMusic : MonoBehaviour {
	
	//Fireball Variables
	public int maxBalls; //Equal to the number of fireballs spawned in Start()
	public float speed; //This float will be multiplied by a Vector3 to deterimine launch speed and angle
	//private int[] nextFireball; //Which index of fireBallsArray will get launched next?
	private GameObject[] fireballsArray; //A 1D array of fireballs.
	private Rigidbody[] rigidbodyArray;  //A 1D array of fireballs' rigidbodies.
	private GameObject fireballSpawner;  //This is just an empty game object nested inside of our character 
	public GameObject MainCam;
	//that sets the spawn point for launched fireballs
	
	
	//Rainbow Variables (Just copy paste all this stuff, it's a bit too tricky to explain right now)
	public float[] rgb;
	public float ballsPerPhase;
	public  float skipThisMany;
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
	
	AudioSource aud;


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
		
		aud = GetComponent<AudioSource>();

		InvokeRepeating("ScaleToSound", 1.0f, .05f);
	}

	protected Vector3 Seek (Vector3 targetPos)
	{
		//find dv, desired velocity
		dv = targetPos - transform.position;		
		dv = dv.normalized; 	//scale by maxSpeed
		dv -= velocity;
		return dv;
	}

	void ScaleToSound()
	{
		float[] spectrum = aud.GetSpectrumData(4096, 0, FFTWindow.Blackman);
		conversion = spectrum.Length/(maxBalls/2);
		float temp = 0;
		
		for (int i = 0; i < maxBalls; i++) {
				if(i == 0){
					temp = 0;
					temp += spectrum[i];
					//temp /= conversion;
					
					temp *= (scaleAmount/conversion);
					temp *=	i;//*conversion * (i*3);
					
					if (temp < 1f)
						temp = 1f;
					
					if(temp > 10)
						temp = 10;
					
					fireballsArray[i].transform.localScale = new Vector3(0,0,0) + Vector3.Lerp(fireballsArray[i].transform.localScale, new Vector3(fireballsArray[i].transform.localScale.x,temp,fireballsArray[i].transform.localScale.z), Time.deltaTime * lerpSpeed);
					fireballsArray[i].renderer.material.color = Color.Lerp(Color.black, new Color((1-Mathf.Pow(fireballsArray[i].transform.localScale.y,colorIntensity) * 1.2f), 0, 0), lerpSpeed);
					//fireballsArray[i,k].renderer.material.color = Color.Lerp(Color.black, new Color(fireballsArray[i,k].transform.localScale.x, 0, 0), lerpSpeed);
				}
				else
				{
					fireballsArray[i].transform.localScale = Vector3.Lerp(fireballsArray[i].transform.localScale, new Vector3(fireballsArray[i-1].transform.localScale.x,fireballsArray[i-1].transform.localScale.y,fireballsArray[i-1].transform.localScale.z), 0.75f);
					fireballsArray[i].renderer.material.color = new Color((1-Mathf.Pow(fireballsArray[i].transform.localScale.y,colorIntensity) * 1.2f),0,0);
					//fireballsArray[i,k].renderer.material.color = Color.Lerp(Color.black, new Color(fireballsArray[i,k].transform.localScale.x, 0, 0), lerpSpeed);
				}
			}		
		}

	// Update is called once per frame
	void Update () {
		Camera.main.GetComponent<SmoothFollow> ().target = fireballsArray[0].transform;

		Debug.Log(Input.mousePosition.x + fireballsArray [0].transform.position.x + " " + Input.mousePosition.y + fireballsArray [0].transform.position.y);

		Vector3 test = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);


		for (int i = 0; i < fireballsArray.Length; i++) {
			if( i < 1)	
				fireballsArray[i].transform.position = Vector3.Lerp (fireballsArray [i].transform.position, test, lerpSpeed/1000 * Time.deltaTime);
			else
				fireballsArray[i].transform.position = Vector3.Lerp(fireballsArray[i].transform.position, fireballsArray[i-1].transform.position, lerpSpeed * Time.deltaTime);
		}


	}
}
