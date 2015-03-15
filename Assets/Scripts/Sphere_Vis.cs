using UnityEngine;
using System.Collections;

public class Sphere_Vis : MonoBehaviour {



	private GameObject[,] boxArray;


	private int numCircles;
	public int numBoxes;
	public int radius;

	public float margin;

	public int rows;
	public int colums;

	public float scaleAmount;
	public float ScaleHarshness;
	private int conversion;
	public float lerpSpeed;
	public float colorIntensity;

	public AudioSource aud;

	public int lowCut;
	public int highCut;

	// Use this for initialization
	void Start () {
		float x, y, z;
		float angle;
		float angle2 = 0;
		float p1,p2;

		numCircles = colums * rows;

		//boxArray = new GameObject[numBoxes*numBoxes];
		boxArray = new GameObject[numCircles,numBoxes];
		for(int k = 0; k < numCircles; k++)
		{
			boxArray[k,0] =(GameObject)Instantiate(Resources.Load("Sphere-Cube"),new Vector3(0, 0, 0), Quaternion.identity);
			((GameObject)(boxArray[k,0])).renderer.enabled = false;
			for(int i = 1; i < numBoxes; i++)
			{
				p1 = (i * 1.0f) / numBoxes;
			
				angle = p1 * Mathf.PI * 2;
//				x = Mathf.Sin(angle)*Mathf.Cos(angle2) * radius;
//				y = Mathf.Sin(angle)*Mathf.Sin(angle2) * radius;
				x = Mathf.Sin (angle) * radius;
				y = Mathf.Cos (angle) * radius;
			
				//w = Mathf.Tan(angle) * radius;
			
//				for(int k = 0; k < numBoxes; k++)
//				{
//					p2 = (k * 1.0f) / numBoxes;
//	  	
//					angle2 = p1 * Mathf.PI * 2;
//					z = Mathf.Cos(angle2) * radius;
//	  	
//					boxArray[i*numBoxes + k] =(GameObject)Instantiate(Resources.Load("Sphere-Cube"), new Vector3(x, y, z), Quaternion.identity); 
//					boxArray[i*numBoxes + k].name = "Sphere Box " + i + "," + k;
//				}
			
				boxArray[k,i] =(GameObject)Instantiate(Resources.Load("Sphere-Cube"), new Vector3(x, y, 0), Quaternion.identity); 
				boxArray[k,i].name = "Sphere Box " + k + "," + i;
				boxArray[k,i].transform.parent = boxArray[k,0].transform;
			}
		}

		for (int i = 0; i < rows; i++) {
			for(int k = 0; k < colums; k++)
			{
				boxArray[(i*colums)+k,0].transform.position = new Vector3(i*(2*radius) * margin,k*(2*radius) * margin,-5);
				//boxArray[(i*colums)+k,0].transform.position = Quaternion.AngleAxis(Random.Range(0,10), Vector3.forward) * boxArray[(i*colums)+k,0].transform.position;
				//else
					//boxArray[(i*3)+k,0].transform.position = new Vector3((i*(2*radius)) + margin,(k*(2*radius)) + margin,0);
			}
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
		float[] spectrum = Camera.main.GetComponent<AudioSource>().GetSpectrumData(4096, 0, FFTWindow.Blackman);
		float[] modFreq = new float[highCut-lowCut];
		System.Array.Copy (spectrum, lowCut, modFreq,0,(long)(highCut - lowCut));
		conversion = (highCut-lowCut)/(numBoxes);
		float temp = 0;
		float temp2 = 0;
		float avg = NormalizeVolume (spectrum);

		for(int z = 0; z < numCircles; z++){

		for (int i = 1; i < numBoxes; i++) {
			if(boxArray[z,i].renderer.isVisible){
				temp = 0;
				System.Array.Copy (spectrum, lowCut, modFreq,0,(long)(highCut - lowCut));
				conversion = modFreq.Length/(numBoxes);
				
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
				
				boxArray [z,i].transform.localScale = new Vector3 (0, 0, 0) + Vector3.Lerp (boxArray [z,i].transform.localScale, new Vector3 (temp/16, temp/16, temp), Time.deltaTime * lerpSpeed);
				boxArray [z,i].renderer.material.color = Color.Lerp (Color.black, new Color (Mathf.Clamp(Mathf.Pow(boxArray [z,i].transform.localScale.z,1.2f) * colorIntensity,0,230), 0, 0), lerpSpeed);
				}
			}
		}

	}

	// Update is called once per frame
	void Update () {
		ScaleToSound ();
	}
}
