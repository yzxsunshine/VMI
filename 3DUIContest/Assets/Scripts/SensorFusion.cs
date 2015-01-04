using UnityEngine;
using System;
using System.Collections;

public class SensorFusion {
	public Vector3 accel;
	public Vector3 gyro;
	public Vector3 magnet;
	public Vector3 orientation;

	private float[] rotationMatrix;
	private float[] gyroMatrix;
	private int initState = 0;
	public Vector3 accMagOrientation;
	private Vector3 gyroOrientation;
	private float timestamp = 0.0f;
	private float FILTER_COEFFICIENT = 0.98f;
#if !UNITY_EDITOR
	private AndroidJavaClass cls_SensorFusion;
#endif
	// Use this for initialization
	public void Start () {
		#if !UNITY_EDITOR
		AndroidJNI.AttachCurrentThread();
		using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
				cls_SensorFusion = new AndroidJavaClass("wpi.hive.vmi.SensorFusionActivity");
				//cls_SensorFusion.CallStatic("Init", obj_Activity);
				//orientation.z = 333.33f;
				cls_SensorFusion.CallStatic("setCoefficient", 0.7f);
			}
		}
#endif
	}

	// Update is called once per frame
	public void Update () {
		#if !UNITY_EDITOR
		cls_SensorFusion.CallStatic("fusion");
		accel.x = cls_SensorFusion.CallStatic<float>("getAcclX");
		accel.y = cls_SensorFusion.CallStatic<float>("getAcclY");
		accel.z = cls_SensorFusion.CallStatic<float>("getAcclZ");

		gyro.x = cls_SensorFusion.CallStatic<float>("getGyroX");
		gyro.y = cls_SensorFusion.CallStatic<float>("getGyroY");
		gyro.z = cls_SensorFusion.CallStatic<float>("getGyroZ");

		magnet.x = cls_SensorFusion.CallStatic<float>("getMagX");
		magnet.y = cls_SensorFusion.CallStatic<float>("getMagY");
		magnet.z = cls_SensorFusion.CallStatic<float>("getMagZ");

		//orientation.x = cls_SensorFusion.CallStatic<float>("getX");
		//orientation.y = cls_SensorFusion.CallStatic<float>("getY");
		orientation.x = cls_SensorFusion.CallStatic<float>("getOrientX");
		orientation.y = cls_SensorFusion.CallStatic<float>("getOrientY");
		orientation.z = cls_SensorFusion.CallStatic<float>("getOrientZ");


#else
		if(Input.acceleration == null)
			return;
		accel = Input.acceleration;
		if(Input.gyro == null)
			return;
		gyro = Input.gyro.rotationRate;
		//gyro = Eular2Rodrigues(Input.gyro.rotationRate);
		if(Input.compass == null)
			return;
		magnet = Input.compass.rawVector;

		CalculateAccMagOrientation();
		GyroFunction();
		CalculateFusedOrientation();
#endif
	}

	public void OnDestroy() {
		#if !UNITY_EDITOR
		cls_SensorFusion.Call("onDestroy");
#endif
	}

	public Vector3 Orientation {
		get{
			return orientation;
		} 
	}


	public void CalculateAccMagOrientation() {
		rotationMatrix = getRotationMatrix(accel, magnet);
		accMagOrientation = getOrientation(rotationMatrix);
		if(initState == 0)
		{
			initState = 1;
		}
	}

	public void GyroFunction() {
		// don't start until first accelerometer/magnetometer orientation has been acquired
		if (accMagOrientation == null || initState == 0)
			return;
		
		// initialisation of the gyroscope based rotation matrix       
		if(initState == 1) {
			float[] initMatrix = new float[9];
			//initMatrix.copyData( getRotationMatrixFromOrientation(accMagOrientation.getData()) );
			Vector3 accMagVec = new Vector3(accMagOrientation.y, accMagOrientation.z, accMagOrientation.x);
			initMatrix = Eular2Matrix3x3(accMagOrientation);
			gyroMatrix = Matrix3x3Mul(gyroMatrix, initMatrix);
			initState = 2;
		}
		
		// copy the new gyro values into the gyro array
		// convert the raw gyro data into a rotation vector
		Quaternion deltaVector = new Quaternion();
		if(timestamp != 0) {
			float dT = (DateTime.Now.Millisecond - timestamp) * 1.0f / 1000;
			gyro = gyro * dT;
			deltaVector = Quaternion.AngleAxis(gyro.magnitude, gyro);
		}
		
		// measurement done, save current time for next interval
		timestamp = DateTime.Now.Millisecond;
		
		// convert rotation vector into rotation matrix
		float[] deltaMatrix = new float[9];

		deltaMatrix = Quaternion2Matrix3x3(deltaVector);
		gyroMatrix = Matrix3x3Mul(gyroMatrix, deltaMatrix);
		// apply the new rotation interval on the gyroscope based rotation matrix
		//gyroMatrix.copyData(matrixMultiplication(gyroMatrix.getData(), deltaMatrix));
		
		// get the gyroscope based orientation from the rotation matrix
		gyroOrientation = getOrientation(gyroMatrix);
	}

	public Vector3 CalculateFusedOrientation() {
		float oneMinusCoeff = 1.0f - FILTER_COEFFICIENT;
		
		/*
             * Fix for 179锟�--> -179锟絫ransition problem:
             * Check whether one of the two orientation angles (gyro or accMag) is negative while the other one is positive.
             * If so, add 360锟�2 * math.PI) to the negative value, perform the sensor fusion, and remove the 360锟絝rom the result
             * if it is greater than 180锟�This stabilizes the output in positive-to-negative-transition cases.
             */
		
		// azimuth
		if (gyroOrientation.x < -0.5f * Math.PI && accMagOrientation.x > 0.0f) {
			float val = (float) (FILTER_COEFFICIENT * (gyroOrientation.x + 2.0f * Mathf.PI) + oneMinusCoeff * accMagOrientation.x);
			val -= (val > Mathf.PI) ? 2.0f * Mathf.PI : 0;
			orientation.x = val;
		}
		else if (accMagOrientation.x < -0.5f * Mathf.PI && gyroOrientation.x > 0.0f) {
			float val = (float) (FILTER_COEFFICIENT * gyroOrientation.x + oneMinusCoeff * (accMagOrientation.x + 2.0f * Mathf.PI)); 
			val -= (val > Mathf.PI)? 2.0f * Mathf.PI : 0;
			orientation.x = val;
		}
		else {
			float val = FILTER_COEFFICIENT * gyroOrientation.x + oneMinusCoeff * accMagOrientation.x;
			orientation.x = val;
		}
		
		// pitch
		if (gyroOrientation.y < -0.5f * Mathf.PI && accMagOrientation.y > 0.0f) {
			float val = (float) (FILTER_COEFFICIENT * (gyroOrientation.y + 2.0f * Mathf.PI) + oneMinusCoeff * accMagOrientation.y);
			val -= (val > Mathf.PI) ? 2.0f * Mathf.PI : 0;
			orientation.y = val;
		}
		else if (accMagOrientation.y < -0.5f * Mathf.PI && gyroOrientation.y > 0.0f) {
			float val = (float) (FILTER_COEFFICIENT * gyroOrientation.y + oneMinusCoeff * (accMagOrientation.y + 2.0f * Mathf.PI));
			val -= (val > Mathf.PI)? 2.0f * Mathf.PI : 0;
			orientation.y = val;
		}
		else {
			float val = FILTER_COEFFICIENT * gyroOrientation.y + oneMinusCoeff * accMagOrientation.y;
			orientation.y = val;
		}
		
		// roll
		if (gyroOrientation.z < -0.5f * Mathf.PI && accMagOrientation.z > 0.0f) {
			float val = (float) (FILTER_COEFFICIENT * (gyroOrientation.z + 2.0f * Mathf.PI) + oneMinusCoeff * accMagOrientation.z);
			val -= (val > Mathf.PI) ? 2.0f * Mathf.PI : 0;
			orientation.z = val;
		}
		else if (accMagOrientation.z < -0.5f * Mathf.PI && gyroOrientation.z > 0.0f) {
			float val = (float) (FILTER_COEFFICIENT * gyroOrientation.z + oneMinusCoeff * (accMagOrientation.z + 2.0f * Mathf.PI));
			val -= (val > Mathf.PI)? 2.0f * Mathf.PI : 0;
			orientation.z = val;
		}
		else {
			float val = FILTER_COEFFICIENT * gyroOrientation.z + oneMinusCoeff * accMagOrientation.z;
			orientation.z = val;
		}
		
		// overwrite gyro matrix and orientation with fused orientation
		// to comensate gyro drift
		gyroMatrix = Eular2Matrix3x3(orientation);
		gyroOrientation.x = orientation.x;
		gyroOrientation.y = orientation.y;
		gyroOrientation.z = orientation.z;
		return orientation;
	}


	public Vector3 getOrientation(float[] R) {
		Vector3 rotVec = new Vector3();
		if (R.Length == 9) {
			rotVec.x = Mathf.Atan2(R[1], R[4]);
			rotVec.y = Mathf.Asin(-R[7]);
			rotVec.z = Mathf.Atan2(-R[6], R[8]);
		} else {
			rotVec.x = Mathf.Atan2(R[1], R[5]);
			rotVec.y = Mathf.Asin(-R[9]);
			rotVec.z = Mathf.Atan2(-R[8], R[10]);
		}
		return rotVec;
	}
	
	public float[] getRotationMatrix(Vector3 gravity, Vector3 geomagnetic) {
		float[] R = new float[9];
		float Ax = gravity.x;
		float Ay = gravity.y;
		float Az = gravity.z;
		float Ex = geomagnetic.x;
		float Ey = geomagnetic.y;
		float Ez = geomagnetic.z;
		float Hx = Ey*Az - Ez*Ay;
		float Hy = Ez*Ax - Ex*Az;
		float Hz = Ex*Ay - Ey*Ax;
		float normH = Mathf.Sqrt(Hx*Hx + Hy*Hy + Hz*Hz);
		if (normH < 0.1f) {
			// device is close to free fall (or in space?), or close to
			// magnetic north pole. Typical values are  > 100.
			return null;
		}
		float invH = 1.0f / normH;
		Hx *= invH;
		Hy *= invH;
		Hz *= invH;
		float invA = 1.0f / Mathf.Sqrt(Ax*Ax + Ay*Ay + Az*Az);
		Ax *= invA;
		Ay *= invA;
		Az *= invA;
		float Mx = Ay*Hz - Az*Hy;
		float My = Az*Hx - Ax*Hz;
		float Mz = Ax*Hy - Ay*Hx;

		R[0] = Hx;     R[1] = Hy;     R[2] = Hz;
		R[3] = Mx;     R[4] = My;     R[5] = Mz;
		R[6] = Ax;     R[7] = Ay;     R[8] = Az;
		return R;
	}

	public float[] Eular2Matrix3x3(Vector3 eular)
	{
		float[] rMat = new float[9];
		float sinA = Mathf.Sin(eular.y);	//pitch
		float cosA = Mathf.Cos(eular.y);
		float sinB = Mathf.Sin(eular.z);	//roll
		float cosB = Mathf.Cos(eular.z);
		float sinH = Mathf.Sin(eular.x);	//yaw
		float cosH = Mathf.Cos(eular.x);

		float[] xM = new float[9];
		float[] yM = new float[9];
		float[] zM = new float[9];
		xM[0] = 1.0f; xM[1] = 0.0f; xM[2] = 0.0f;
		xM[3] = 0.0f; xM[4] = cosA; xM[5] = sinA;
		xM[6] = 0.0f; xM[7] = -sinA; xM[8] = cosA;
		
		// rotation about y-axis (roll)
		yM[0] = cosB; yM[1] = 0.0f; yM[2] = sinB;
		yM[3] = 0.0f; yM[4] = 1.0f; yM[5] = 0.0f;
		yM[6] = -sinB; yM[7] = 0.0f; yM[8] = cosB;
		
		// rotation about z-axis (azimuth)
		zM[0] = cosH; zM[1] = sinH; zM[2] = 0.0f;
		zM[3] = -sinH; zM[4] = cosH; zM[5] = 0.0f;
		zM[6] = 0.0f; zM[7] = 0.0f; zM[8] = 1.0f;
		
		rMat = Matrix3x3Mul(xM, yM);
		rMat = Matrix3x3Mul(xM, rMat);
		return rMat;
	}

	// well it's not appropriate to put so many math function here
	private float[] Matrix3x3Mul(float[] m1, float[] m2) {
		float[] m = new float[9];
		m[0] = m1[0]*m2[0] + m1[1]*m2[3] + m1[2]*m2[6];
		m[1] = m1[0]*m2[1] + m1[1]*m2[4] + m1[2]*m2[7];
		m[2] = m1[0]*m2[2] + m1[1]*m2[5] + m1[2]*m2[8];
		m[3] = m1[3]*m2[0] + m1[4]*m2[3] + m1[5]*m2[6];
		m[4] = m1[3]*m2[1] + m1[4]*m2[4] + m1[5]*m2[7];
		m[5] = m1[3]*m2[2] + m1[4]*m2[5] + m1[5]*m2[8];
		m[6] = m1[6]*m2[0] + m1[7]*m2[3] + m1[8]*m2[6];
		m[7] = m1[6]*m2[1] + m1[7]*m2[4] + m1[8]*m2[7];
		m[8] = m1[6]*m2[2] + m1[7]*m2[5] + m1[8]*m2[8];
		return m;
	}

	public float[] Quaternion2Matrix3x3(Quaternion q)
	{
		float[] m = new float[9];
		m[0] = 1 - 2*q.y*q.y - 2*q.z*q.z;
		m[1] = 2*q.x*q.y - 2*q.z*q.w;
		m[2] = 2*q.x*q.z + 2*q.y*q.w;
		
		m[3] = 2*q.x*q.y + 2*q.z*q.w;
		m[4] = 1 - 2*q.x*q.x - 2*q.z*q.z;
		m[5] = 2*q.y*q.z - 2*q.x*q.w;
		
		m[6] = 2*q.x*q.z - 2*q.y*q.w;
		m[7] = 2*q.y*q.z + 2*q.x*q.w;
		m[8] = 1 - 2*q.x*q.x - 2*q.y*q.y;
		return m;
	}

	public static Vector3 Eular2Rodrigues(Vector3 vec)
	{
		Vector3 rod = new Vector3();
		float c1 = Mathf.Cos(vec.x/2);
		float s1 = Mathf.Sin(vec.x/2);
		float c2 = Mathf.Cos(vec.y/2);
		float s2 = Mathf.Sin(vec.y/2);
		float c3 = Mathf.Cos(vec.z/2);
		float s3 = Mathf.Sin(vec.z/2);
		float c1c2 = c1*c2;
		float s1s2 = s1*s2;
		float w =c1c2*c3 - s1s2*s3;
		rod.x = c1c2*s3 + s1s2*c3;
		rod.y = s1*c2*c3 + c1*s2*s3;
		rod.z = c1*s2*c3 - s1*c2*s3;
		float angle = 2 * Mathf.Acos(w);
		rod.Normalize();
		rod = rod * angle;
		return rod;
	}
}
