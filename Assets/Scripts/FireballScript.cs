// @Author:		Nate Danziger
// Please feel free to share this code, or even use it in your own projects! :)

using UnityEngine;
using System.Collections;

public class FireballScript : MonoBehaviour {
	
	//Fireball Variables
	public int maxBalls; //Equal to the number of fireballs spawned in Start()
	public float speed; //This float will be multiplied by a Vector3 to deterimine launch speed and angle
	//private int[] nextFireball; //Which index of fireBallsArray will get launched next?
	private GameObject[,] fireballsArray; //A 1D array of fireballs.
	private Rigidbody[,] rigidbodyArray;  //A 1D array of fireballs' rigidbodies.
	private GameObject fireballSpawner;  //This is just an empty game object nested inside of our character 
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
	public int lerpSpeed;

	public float noiseThreshold;

	AudioSource aud;

	//Start() happens once while the scene is loading
	void Start () {



		dispActual = 0;
		//nextFireball = new int[2];

		fireballsArray = new GameObject[maxBalls/2,maxBalls/2]; //Sets how big our 1D array of GameObjects is
		rigidbodyArray = new Rigidbody[maxBalls/2,maxBalls/2]; //We're putting these components in their own array because using GetComponent at runtime is relatively slow
		for (int i = 0; i < maxBalls/2; i++){ //Iterates through every fireball reference, instantiating a prefab and assigning it a GameObject reference in our 1D array 
			for(int k = 0; k < maxBalls/2; k++){
				fireballsArray[i,k] = (GameObject)Instantiate(Resources.Load("fireballPrefab"), new Vector3(i*3, -20, k*3), Quaternion.identity); 
				fireballsArray[i,k].name = "Fireball " + i + "," + k; //Names the prefab (visible in hierarchy window) 
				rigidbodyArray[i,k] = fireballsArray[i,k].GetComponent<Rigidbody>(); //Assigns our rigidbody REFERENCE IN THE ARRAY to look up this specific fireball's rigidbody
			}
		}
		
		fireballSpawner = GameObject.Find("spawner"); //GameObject.Find should usually only be used in Start(), it's a bit to slow to be used in Update() 										  
		//or anything that repeats like InvokeRepeating
//		nextFireball [0] = 0; 
//		nextFireball [1] = 0;
		
		//PaintFireballs(); //Paints the fireballs rainbow... NOTE: This method only works correctly with 15 fireballs or more 
		
		//InvokeRepeating("CheckButtons", 1.0f, .05f); //CheckButtons() is used instead of Update() to limit the rate of how fast balls were firing.
		InvokeRepeating("ScaleToSound", 1.0f, .05f);
		InvokeRepeating("MoveToSound", 1.0f, .05f);

		foreach (string device in Microphone.devices) {
			Debug.Log("Name: " + device);
		}

		aud = GetComponent<AudioSource>();
//		aud.clip = Microphone.Start("Built-in Microphone", true, 10, 44100);
//		audio.mute = true;
//		audio.loop = true;
//		aud.Play();



	}

	void ScaleToSound()
	{
		float[] spectrum = aud.GetSpectrumData(4096, 0, FFTWindow.Blackman);
		conversion = spectrum.Length/(maxBalls/2);
		float temp = 0;

		for (int i = 0; i < maxBalls/2; i++) {

			for(int k = 0; k < maxBalls/2; k++){
				if(k == 0){
					temp = 0;
					temp += spectrum[i];
					//temp /= conversion;
					
					temp *= (scaleAmount/conversion);
					temp *=	i;//*conversion * (i*3);
					
					if (temp < 1f)
						temp = 1f;
					
					if(temp > 10)
						temp = 10;
					
					fireballsArray[i,k].transform.localScale = new Vector3(0,0,0) + Vector3.Lerp(fireballsArray[i,k].transform.localScale, new Vector3(temp,temp,temp), Time.deltaTime * lerpSpeed);
				}
				else
				{
					fireballsArray[i,k].transform.localScale = Vector3.Lerp(fireballsArray[i,k].transform.localScale, new Vector3(fireballsArray[i,k-1].transform.localScale.x,fireballsArray[i,k-1].transform.localScale.y,fireballsArray[i,k-1].transform.localScale.z), 0.75f);
				}
			}		
		}
	}

	void MoveToSound()
	{
		float[] spectrum = aud.GetSpectrumData(4096, 0, FFTWindow.Blackman);
		conversion = spectrum.Length/(maxBalls/2);
		//float temp = 0;
		Vector3 temp = Vector3.zero;

		for (int i = 0; i < maxBalls/2; i++) {

			for(int k = 0; k < maxBalls/2; k++){
				if(k == 0){
					temp = Vector3.zero;
//					temp = 0;
//					temp += spectrum[i];
//					//temp /= conversion;
//					
//					temp *= dispAmount;
//					temp *=	i;//*conversion * (i*3);
					if(spectrum[i] > noiseThreshold){
						temp = new Vector3(0,spectrum[i] * dispAmount * (maxBalls/2 - k),0);
						temp /= conversion;
					}
					//Vector3.ClampMagnitude(temp,UpperBound);
					
					if (fireballsArray[i,k].transform.position.y + temp.y < LowerBound)
						temp.y = LowerBound - fireballsArray[i,k].transform.position.y + temp.y;
					
					if (fireballsArray[i,k].transform.position.y + temp.y > UpperBound)
						temp.y = UpperBound - fireballsArray[i,k].transform.position.y + temp.y;
					
					fireballsArray[i,k].transform.position = Vector3.Lerp(fireballsArray[i,k].transform.position,fireballsArray[i,k].transform.position + temp, Time.deltaTime * lerpSpeed);
					
					//fireballsArray[i,k].renderer.material.color = Color.Lerp(Color.black, new Color(temp.y, temp.y, temp.y), lerpSpeed);
				}
				else{
					//if(i > 0)
					fireballsArray[i,k].transform.position = Vector3.Lerp(fireballsArray[i,k].transform.position, new Vector3(fireballsArray[i,k].transform.position.x,fireballsArray[i,k-1].transform.position.y,fireballsArray[i,k].transform.position.z), 0.75f);
					//fireballsArray[i,k].renderer.material.color = Color.Lerp(fireballsArray[i,k].renderer.material.color,fireballsArray[i,k-1].renderer.material.color, Time.deltaTime*lerpSpeed);
				}
					if (fireballsArray[i,k].transform.position.y - 2 < LowerBound)
					fireballsArray[i,k].transform.position -= new Vector3(0,-16,0);
			}

		}
	}

	void Update() //is called once per frame
	{
		//aud.Stop ();
		//aud.Play();

		//audio.pitch two butons up and down
		if (Input.GetKey (KeyCode.Q)) {
			audio.pitch -= 0.01f;
		}
		if (Input.GetKey (KeyCode.E)) {
			audio.pitch += 0.01f;
		}

	}

//	void CheckButtons () {
//		
//		if(Input.GetMouseButton(0)){
//			
//			fireballsArray[nextFireball[0],nextFireball[1]].transform.position = fireballSpawner.transform.position; //This fireball is now located at fireballSpawner's position
//			fireballsArray[nextFireball[0],nextFireball[1]].transform.rotation = fireballSpawner.transform.rotation; //This fireball is now rotated to fireballSpawner's rotation
//			
//			rigidbodyArray[nextFireball[0],nextFireball[1]].useGravity = true; //This fireball will now have gravity turned on for it
//			rigidbodyArray[nextFireball[0],nextFireball[1]].velocity = fireballSpawner.transform.forward * speed; //This fireball will now launch at (fireballSpawner.transform.forward * speed)
//
//			if(nextFireball[1] >= maxBalls/2){
//				nextFireball[1] = 0;
//				nextFireball[0] += 1;
//			}
//			//nextFireball++; //Let's use the next fireball next time
//			if(nextFireball[0] >= maxBalls/2){nextFireball[0] = 0;} //If we are out of the range of our firballsArray, reset nextFireball to 0
//			
//		}
//	}
	
	/// <summary>
	/// This method, along with RainbowHelper, sets each fireball to be a different color, creating a rainbow effect.
	/// By "paint fireballs" I actually mean "set fireball's material color", but that doesn't roll of the tongue as well.
	/// </summary>
//	void PaintFireballs()
//	{
//		rgb = new float[3];
//		rgb[0] = 1.0f;
//		rgb[1] = 0;
//		rgb[2] = 0;
//		
//		rainbowIndex = 0;
//		ballsPerPhase = maxBalls / 6.0f;
//		skipThisMany = 1.0f / ballsPerPhase;
//		
//		RainbowHelper(1, true);
//		RainbowHelper(0, false);
//		RainbowHelper(2, true);
//		RainbowHelper(1, false);
//		RainbowHelper(0, true);
//		RainbowHelper(2, false);
//		
//		while (rainbowIndex < maxBalls){fireballsArray[rainbowIndex].renderer.material.color = new Color(rgb[0], rgb[1], rgb[2]); rainbowIndex++;}
//	}
//	
//	/// <summary>
//	/// This method, along with RainbowHelper, sets each fireball to be a different color, creating a rainbow effect.
//	/// </summary>
//	void RainbowHelper(int rgbIndex, bool upTrue){
//		
//		for(int j = 0; j < ballsPerPhase - 1; j++){
//			fireballsArray[rainbowIndex].renderer.material.color = new Color(rgb[0], rgb[1], rgb[2]);
//			fireballsArray[rainbowIndex].GetComponent<ParticleSystem>().startColor = new Color(rgb[0], rgb[1], rgb[2]);
//			if(upTrue){rgb[rgbIndex] += skipThisMany;}
//			else{rgb[rgbIndex] -= skipThisMany;}
//			rainbowIndex++;
//		}
//	}
}
