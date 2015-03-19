using UnityEngine;
using System.Collections;

public class Sphere_Vis : MonoBehaviour {



	private GameObject[] boxArray;

	public int numBoxes;
	public int radius;

	public float scaleAmount;
	public float ScaleHarshness;
	private int conversion;
	public float lerpSpeed;
	public float colorIntensity;

	public AudioSource aud;

	// Use this for initialization
	void Start () {
		float x, y, z;
		float angle;
		float angle2 = 0;
		float p1,p2;



		//boxArray = new GameObject[numBoxes*numBoxes];
		boxArray = new GameObject[numBoxes];
		for(int i = 0; i < numBoxes; i++)
		{
			p1 = (i * 1.0f) / numBoxes;

			angle = p1 * Mathf.PI * 2;
//			x = Mathf.Sin(angle)*Mathf.Cos(angle2) * radius;
//			y = Mathf.Sin(angle)*Mathf.Sin(angle2) * radius;
			x = Mathf.Sin (angle) * radius;
			y = Mathf.Cos (angle) * radius;

			//w = Mathf.Tan(angle) * radius;

//			for(int k = 0; k < numBoxes; k++)
//			{
//				p2 = (k * 1.0f) / numBoxes;
//
//				angle2 = p1 * Mathf.PI * 2;
//				z = Mathf.Cos(angle2) * radius;
//
//				boxArray[i*numBoxes + k] =(GameObject)Instantiate(Resources.Load("Sphere-Cube"), new Vector3(x, y, z), Quaternion.identity); 
//				boxArray[i*numBoxes + k].name = "Sphere Box " + i + "," + k;
//			}

			boxArray[i] =(GameObject)Instantiate(Resources.Load("Sphere-Cube"), new Vector3(x, y, 0), Quaternion.identity); 
			boxArray[i].name = "Sphere Box " + i;
		}

		aud = Camera.main.GetComponent<AudioSource>();
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
		conversion = spectrum.Length/(boxArray.Length);
		float temp = 0;
		float temp2 = 0;
		float avg = NormalizeVolume (spectrum);
		
		for (int i = 0; i < boxArray.Length; i++) {
			temp = 0;
			
			for (int k = 0; k < conversion; k++) {
				temp2 = 0;
				temp2 += spectrum [(i * conversion) + k];
				//temp /= conversion;
				//				if (temp2 < avg)
				//						temp2 /= avg;
				//				else {
				//						temp2 *= avg;
				//				}
				
				temp2 *= scaleAmount * ((i * conversion));
				temp+= temp2;
			}
			
			temp /= conversion;
			
			
			//temp = Mathf.Clamp(temp,0.2f,8);
			
			temp *= 10;
			
			temp = Mathf.Pow(temp, ScaleHarshness);
			
			temp /= 10;
			
			temp = Mathf.Clamp(temp,0.2f,10);
			
			boxArray [i].transform.localScale = new Vector3 (0, 0, 0) + Vector3.Lerp (boxArray [i].transform.localScale, new Vector3 (temp, temp, temp), Time.deltaTime * lerpSpeed);
			boxArray [i].renderer.material.color = Color.Lerp (Color.black, new Color ((1 - Mathf.Pow (temp* -colorIntensity, colorIntensity) * 1.2f), 0, 0), lerpSpeed);
		}
	}

	// Update is called once per frame
	void Update () {
		ScaleToSound ();
	}
}
