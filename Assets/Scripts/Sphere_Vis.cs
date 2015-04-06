using UnityEngine;
using System.Collections;

public class Sphere_Vis : MonoBehaviour {



	private GameObject[,] boxArray;
	private GameObject[] borders;
	private Vector3[] ogPosition;


	private int numCircles;
	public int numBoxes;
	public float radius;

	public float margin;
	public float circleLerp;

	public float travelDist;
	public float fromCamDist;

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

	public float noiseThreshold;
	public float rotationSpeed;

	// Use this for initialization
	void Start () {
		float x, y, z;
		float angle;
		float angle2 = 0;
		float p1,p2;

		numCircles = colums * rows;

		borders = new GameObject[4];

		//boxArray = new GameObject[numBoxes*numBoxes];
		boxArray = new GameObject[numCircles,numBoxes];
		ogPosition = new Vector3[numCircles];
		for(int k = 0; k < numCircles; k++)
		{
			boxArray[k,0] =(GameObject)Instantiate(Resources.Load("Sphere-Cube"),new Vector3(0, 0, 0), Quaternion.identity);
			((GameObject)(boxArray[k,0])).renderer.enabled = false;
			for(int i = 1; i < numBoxes; i++)
			{
				p1 = (i+1 * 1.0f) / numBoxes-1;
			
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
				boxArray[(i*colums)+k,0].transform.position = new Vector3(i*(2*radius) * margin,k*(2*radius) * margin,-0.5f);
				boxArray[(i*colums)+k,0].transform.RotateAround(boxArray[(i*colums)+k,0].transform.position,Vector3.forward,Random.Range(0,10000));
				ogPosition[(i*colums)+k] = boxArray[(i*colums)+k,0].transform.position;
				//boxArray[(i*colums)+k,0].transform.position = Quaternion.AngleAxis(Random.Range(0,10), Vector3.forward) * boxArray[(i*colums)+k,0].transform.position;
				//else
					//boxArray[(i*3)+k,0].transform.position = new Vector3((i*(2*radius)) + margin,(k*(2*radius)) + margin,0);
			}
		}


		borders[0] = (GameObject)Instantiate(Resources.Load("Border"), new Vector3(rows/2 *(2*radius) * margin - 2*margin, colums*(2*radius) * margin - 2 * margin, -5.999998f), Quaternion.identity);
		borders[0].transform.localScale = new Vector3(rows*(2*radius) * margin,1,1);
		borders[0].name = "Border " + 0;

		borders[1] = (GameObject)Instantiate(Resources.Load("Border"), new Vector3(rows/2 *(2*radius) * margin,-4f, -5.999998f), Quaternion.identity);
		borders[1].transform.localScale = new Vector3(rows*(2*radius) * margin,1,1);
		borders[1].name = "Border " + 1;

		borders[2] = (GameObject)Instantiate(Resources.Load("Border"), new Vector3(-4f,rows/2 *(2*radius) * margin, -5.999998f), Quaternion.identity);
		borders[2].transform.localScale = new Vector3(1,colums*(2*radius) * margin,1);
		borders[2].name = "Border " + 2;

		borders[3] = (GameObject)Instantiate(Resources.Load("Border"), new Vector3(colums *(2*radius) * margin,rows/2 *(2*radius) * margin, -5.999998f), Quaternion.identity);
		borders[3].transform.localScale = new Vector3(1,colums*(2*radius) * margin,1);
		borders[3].name = "Border " + 3;
		
		
		aud = Camera.main.GetComponent<AudioSource>();
		aud.time = 60;
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

		if (spectrum [(int)(spectrum.Length * 0.9)] > noiseThreshold) {
			for (int i = 0; i < rows; i++) {
				for (int k = 0; k < colums; k++) {
					//boxArray[(i*colums)+k,0].transform.position = new Vector3(i*(2*radius) * margin,k*(2*radius) * margin,-0.5f);
					boxArray [(i * colums) + k, 0].transform.RotateAround (boxArray [(i * colums) + k, 0].transform.position, Vector3.forward, rotationSpeed);
					//ogPosition[(i*colums)+k] = boxArray[(i*colums)+k,0].transform.position;
					//boxArray[(i*colums)+k,0].transform.position = Quaternion.AngleAxis(Random.Range(0,10), Vector3.forward) * boxArray[(i*colums)+k,0].transform.position;
					//else
					//boxArray[(i*3)+k,0].transform.position = new Vector3((i*(2*radius)) + margin,(k*(2*radius)) + margin,0);
				}
			}
		}

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
				
				boxArray [z,i].transform.localScale = new Vector3 (0, 0, 0) + Vector3.Lerp (boxArray [z,i].transform.localScale, new Vector3 (temp/(numBoxes/3f), temp/(numBoxes/3f), temp), Time.deltaTime * lerpSpeed);
				
				//if(Random.Range (0,2)  == 0)
				if(z%2  == 0)
					boxArray [z,i].renderer.material.color = Color.Lerp (Color.black, new Color (Mathf.Clamp(Mathf.Pow(boxArray [z,i].transform.localScale.z,1.2f) * colorIntensity,0,230), 0, 0), lerpSpeed);
				else
						boxArray [z,i].renderer.material.color = Color.Lerp (Color.black, new Color (0, 0, Mathf.Clamp(Mathf.Pow(boxArray [z,i].transform.localScale.z,1.2f) * colorIntensity,0,230)), lerpSpeed);
				}
			}
		}
	}

	void MoveToCam()
	{
		float distToCam;
		float distFromOG;
		Vector3 toCam;
		
		for(int i = 0; i < numCircles; i++)
		{
			if(boxArray[i,1].renderer.isVisible){
				toCam = Camera.main.transform.position - boxArray[i,0].transform.position;
				toCam /= toCam.magnitude;
				distToCam =  Vector3.Distance(Camera.main.transform.position, boxArray[i,0].transform.position);
				distFromOG = Vector3.Distance(boxArray[i,0].transform.position, ogPosition[i]);
				
				if(distToCam <= fromCamDist){
					//boxArray[i,0].transform.position = Vector3.Lerp(boxArray[i,0].transform.position,Camera.main.transform.position,(circleLerp*distToCam) * Time.deltaTime);
					//boxArray[i,0].transform.position = Vector3.Lerp(boxArray[i,0].transform.position,boxArray[i,0].transform.position + (toCam * distToCam),(circleLerp*distToCam) * Time.deltaTime);
					boxArray[i,0].transform.position = new Vector3(0,0,boxArray[i,0].transform.position.z-(0.2f * distToCam));
				}
				else{
					//boxArray[i,0].transform.position = Vector3.Lerp(boxArray[i,0].transform.position,ogPosition[i],circleLerp * Time.deltaTime);
					boxArray[i,0].transform.position = ogPosition[i];
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
		ScaleToSound ();
		//MoveToCam ();
	}
}
