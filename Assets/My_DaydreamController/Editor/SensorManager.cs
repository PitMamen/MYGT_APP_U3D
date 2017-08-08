using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SensorManager : MonoBehaviour
{
	public static SensorManager instance;

	public SensorModules sensorModule = SensorModules.spacePointRaw;
	public float updateRate = 60.0f;

	public static Quaternion rotationAbsolute = Quaternion.identity;
	public static Quaternion rotationRelativeToScreen = Quaternion.identity;
	public static Vector3 measuredAcceleration = Vector3.zero;
	public static Vector3 nonGravityAcceleration = Vector3.zero;
	public static Vector2 cursorAbsolute;
	public static Vector2 cursorRelative;

   	private Quaternion PNItoUnity = Quaternion.Euler(0, 180, 0);
	
	
	public static float GetData (Signals signal)
	{
		if (instance != null) {
			return instance.rawData[(int)signal];
		}
		return 0.0f;
	}

	public enum SensorModules
	{
		spacePointEmbedded = 0,
		spacePointRaw
	}

	static Quaternion centerOfScreenRotation = Quaternion.identity;

	public enum Signals
	{
		accX = 0,
		accY,
		accZ,
		magX,
		magY,
		magZ,
		gyrX,
		gyrY,
		gyrZ,
		qx,
		qy,
		qz,
		qw,
		ngaX,
		ngaY,
		ngaZ,
		Length
	}

	float[] rawData = new float[(int)Signals.Length];

	void Awake ()
	{
		if (instance == null) {
			Debug.Log("New SensorManager");	
			instance = this;
			DontDestroyOnLoad(gameObject);
			//restore previous screen setting
			//centerOfScreenRotation = Quaternion.Euler (PlayerPrefs.GetFloat ("screenX"), PlayerPrefs.GetFloat ("screenY"), PlayerPrefs.GetFloat ("screenZ"));
			//fsdk = GetComponent
			Debug.Log("Starting SensorManagerUpdate");
			centerOfScreenRotation = PNItoUnity;
			Debug.Log (centerOfScreenRotation);
			InvokeRepeating ("SensorManagerUpdate", 0.0f, 1.0f / updateRate);			
		} else {
			Destroy(gameObject);
		}
	}
		
	void SensorManagerUpdate ()
	{
		int i = 0;
		switch (sensorModule) {
		case SensorModules.spacePointEmbedded:
			//store measured data
			//SP embedded X-Y-Z is right handed, x-forward, z-down +/-6G -> +/- 1
			measuredAcceleration = 6.0f*(new Vector3(Input.GetAxisRaw ("sp2"),-Input.GetAxisRaw ("sp3"),Input.GetAxisRaw("sp1")));
			
			
			rotationAbsolute.x = -Input.GetAxisRaw ("sp5");
			rotationAbsolute.y = Input.GetAxisRaw ("sp6");
			rotationAbsolute.z = -Input.GetAxisRaw ("sp4");
			rotationAbsolute.w = Input.GetAxisRaw ("sp7");
			Debug.Log("Sp1: "+Input.GetAxisRaw("sp1"));
			for (i = 0; i < 3; i++){
				rawData[i] = measuredAcceleration[i];
			}
			nonGravityAcceleration = Vector3.up + rotationAbsolute*measuredAcceleration;
			
			//nga
			for (i = 0; i < 3; i++) {
				rawData[i+13] = nonGravityAcceleration[i];
			}
			
			break;
		case SensorModules.spacePointRaw:
			rotationAbsolute.x = -SpacePointAlgorithm.api_output.quaternion[1];
			rotationAbsolute.y = SpacePointAlgorithm.api_output.quaternion[2];
			rotationAbsolute.z = -SpacePointAlgorithm.api_output.quaternion[0];
			rotationAbsolute.w = SpacePointAlgorithm.api_output.quaternion[3];
		
			measuredAcceleration.x = SpacePointAlgorithm.api_output.Accelerate[1]+SpacePointAlgorithm.api_output.Gravity[1];			
			measuredAcceleration.y = -(SpacePointAlgorithm.api_output.Accelerate[2]+SpacePointAlgorithm.api_output.Gravity[2]);			
			measuredAcceleration.z = SpacePointAlgorithm.api_output.Accelerate[0]+SpacePointAlgorithm.api_output.Gravity[0];
			
			nonGravityAcceleration = Vector3.up + rotationAbsolute*measuredAcceleration;
//			//mag
//			for (i = 0; i < 3; i++) {
//				rawData[i+3] = SpacePointAlgorithm.api_input.mag_cal[i];
//			}
//			
//			//acc
//			for (i = 0; i < 3; i++){
//				rawData[i] = SpacePointAlgorithm.api_input.acc_raw[i];
//			}
//			
//			//nga
//			for (i = 0; i < 3; i++) {
//				rawData[i+13] = SpacePointAlgorithm.api_output.Accelerate[i];
//			}
			break;
			
		}
		rotationRelativeToScreen = centerOfScreenRotation * rotationAbsolute;
		
		//absoluteRotation
		for (i = 0; i < 4; i++) {
			rawData[i+9] = rotationAbsolute[i];
		}
		
		
		//Cursor position on screen
//		Vector3 forward = rotationRelativeToScreen * Vector3.forward;		
//		if(forward.z>0){		
//			cursorAbsolute = new Vector2 (forward.x, -forward.y);
//		}
//		else{
//			// this keeps |x| greater than 1 if pointed away from the screen
//			cursorAbsolute = new Vector2 (Mathf.Sign(forward.x)*(2.0f-Mathf.Abs(forward.x)), -forward.y);
//		}
	}

	public static void SetScreenHeading ()
	{
		Debug.Log ("Setting Screen Heading");
		centerOfScreenRotation = Quaternion.Inverse(rotationAbsolute);		
		
//		PlayerPrefs.SetFloat ("screenX", centerOfScreenRotation.eulerAngles.x);
//		PlayerPrefs.SetFloat ("screenY", centerOfScreenRotation.eulerAngles.y);
//		PlayerPrefs.SetFloat ("screenZ", centerOfScreenRotation.eulerAngles.z);
		
		
		Debug.Log (centerOfScreenRotation);
	}
}
