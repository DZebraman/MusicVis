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

	public float ScaleHarshness;
	
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
	public float followSpeed;
	
	public float colorIntensity;

	public int highCut;
	public int lowCut;

	public float bassThreshold;
	
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

			if(i > 0){
				fireballsArray[i] = (GameObject)Instantiate(Resources.Load("fireballPrefab"), new Vector3(i*3, 0, 0), Quaternion.identity); 
				fireballsArray[i].name = "Snake " + i; //Names the prefab (visible in hierarchy window) 
				rigidbodyArray[i] = fireballsArray[i].GetComponent<Rigidbody>(); //Assigns our rigidbody REFERENCE IN THE ARRAY to look up this specific fireball's rigidbody
			}
			else
			{
				fireballsArray[i] = (GameObject)Instantiate(Resources.Load("Snake-Leader"), new Vector3(i*3, 0, 0), Quaternion.identity); 
				fireballsArray[i].name = "Leader " + i; //Names the prefab (visible in hierarchy window) 
				rigidbodyArray[i] = fireballsArray[i].GetComponent<Rigidbody>();
			}
		}
		
		fireballsArray [0].GetComponent<Renderer> ().material.color = Color.black;

		//InvokeRepeating("ScaleToSound", 1.0f, .05f);
		aud = GetComponent<AudioSource>();
		//aud.time = 60;
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
		float[] spectrum = Camera.main.GetComponent<AudioSource>().GetSpectrumData(4096, 0, FFTWindow.Blackman);
		float[] modFreq = new float[highCut-lowCut];
		System.Array.Copy (spectrum, lowCut, modFreq,0,(long)(highCut - lowCut));
		conversion = (highCut-lowCut)/(maxBalls);
		float temp = 0;
		float temp2 = 0;
		float avg = NormalizeVolume (spectrum);
		
		for (int i = 1; i < maxBalls; i++) {
				if(fireballsArray[i].GetComponent<Renderer>().isVisible){
					temp = 0;
					System.Array.Copy (spectrum, lowCut, modFreq,0,(long)(highCut - lowCut));
					conversion = modFreq.Length/(maxBalls);
					
					for (int k = 0; k < conversion; k++) {
						temp2 = modFreq [(i * conversion) + k];
						temp /= conversion;
						
						//avg = NormalizeVolume (spectrum);
						
						//temp2 *= avg;
						
						temp2 *= scaleAmount * Mathf.Log(temp2) * ((i * conversion) + k);
						if(temp2 < 0)
							temp2 *= -1;
						temp+= temp2;
					}
					
					temp /= conversion;
					//Debug.Log(temp);
					
					//temp = Mathf.Clamp(temp,0.2f,8);
					
					temp *= 10;
					
					temp = Mathf.Pow(temp, ScaleHarshness);
					
					temp /= 10;
					
					temp = Mathf.Clamp(temp,0.2f,80);
					
					fireballsArray [i].transform.localScale = new Vector3 (0, 0, 0) + Vector3.Lerp (fireballsArray[i].transform.localScale, new Vector3 (temp, temp, temp), Time.deltaTime * lerpSpeed);

					fireballsArray [i].GetComponent<Renderer>().material.color = Color.Lerp (Color.black, new Color (Mathf.Clamp(Mathf.Pow(fireballsArray[i].transform.localScale.z,1.2f) * colorIntensity,0,230), 0, 0), lerpSpeed);

					if (temp > 6) {
								fireballsArray [i].GetComponent<ParticleSystem> ().enableEmission = true;
									fireballsArray [i].GetComponent<ParticleSystem> ().startSpeed = fireballsArray [i].transform.localScale.y * 3;
									fireballsArray [i].GetComponent<ParticleSystem> ().startLifetime = fireballsArray [i].transform.localScale.y / 8;
								} else
										fireballsArray [i].GetComponent<ParticleSystem> ().enableEmission = false;
								
				}
			}
		}

//	void ScaleToSound()
//	{
//		float[] spectrum = aud.GetSpectrumData(4096, 0, FFTWindow.Blackman);
//		conversion = spectrum.Length/(maxBalls);
//		float temp = 0;
//		float temp2 = 0;
//		float avg = NormalizeVolume (spectrum);
//
//		for (int i = 1; i < maxBalls; i++) {
//			temp = 0;
//			
//			for (int k = 0; k < conversion; k++) {
//				temp2 = 0;
//				temp2 += spectrum [(i * conversion) + k];
//				//temp /= conversion;
////				if (temp2 < avg)
////						temp2 /= avg;
////				else {
////						temp2 *= avg;
////				}
//
//				temp2 *= scaleAmount * ((i * conversion));
//				temp+= temp2;
//			}
//
//			temp /= conversion;
//
//
//			//temp = Mathf.Clamp(temp,0.2f,8);
//
//			temp *= 10;
//
//			temp = Mathf.Pow(temp, ScaleHarshness);
//
//			temp /= 10;
//
//			temp = Mathf.Clamp(temp,0.2f,10);
//
//			fireballsArray [i].transform.localScale = new Vector3 (0, 0, 0) + Vector3.Lerp (fireballsArray [i].transform.localScale, new Vector3 (temp, temp, temp), Time.deltaTime * lerpSpeed);
//			fireballsArray [i].renderer.material.color = Color.Lerp (Color.gray, new Color ((1 - Mathf.Pow (fireballsArray [i].transform.localScale.y, colorIntensity) * 1.2f), 0, 0), lerpSpeed);
//
//			if (temp > 6) {
//			fireballsArray [i].GetComponent<ParticleSystem> ().enableEmission = true;
//				fireballsArray [i].GetComponent<ParticleSystem> ().startSpeed = fireballsArray [i].transform.localScale.y * 3;
//				fireballsArray [i].GetComponent<ParticleSystem> ().startLifetime = fireballsArray [i].transform.localScale.y / 8;
//			} else
//					fireballsArray [i].GetComponent<ParticleSystem> ().enableEmission = false;
//			}
//		}

	// Update is called once per frame
	void Update () {
	
		Camera.main.GetComponent<SmoothFollow> ().target = fireballsArray[0].transform;
		GameObject light = GameObject.Find("Light");
		light.GetComponent<SmoothFollow>().target = fireballsArray[0].transform;

		Vector3 test = new Vector3 (Input.mousePosition.x , Input.mousePosition.y, 0);


		for(int i = 1; i < fireballsArray.Length; i++)
			fireballsArray[i].transform.position = Vector3.Lerp(fireballsArray[i].transform.position, fireballsArray[i-1].transform.position, followSpeed * Time.deltaTime * 1.3f);

//		float[] spectrum = aud.GetSpectrumData(2048, 0, FFTWindow.Blackman);
//		float bassLoud = 0;
//		for(int i = 0; i < 64; i++)
//		{
//			bassLoud += spectrum[i];
//		}

//		bassLoud /= 64;
//
//		if(bassLoud > bassThreshold){
//			for (int i = 0; i < fireballsArray.Length; i++) {
//				if( i < 1)	
//					fireballsArray[i].transform.position = Vector3.Lerp (fireballsArray [i].transform.position, test, followSpeed/500 * Time.deltaTime);
//				else
//					fireballsArray[i].transform.position = Vector3.Lerp(fireballsArray[i].transform.position, fireballsArray[i-1].transform.position, followSpeed * Time.deltaTime * 1.3f);
//			}
//		}
		ScaleToSound ();
	}
}
