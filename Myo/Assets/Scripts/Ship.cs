using UnityEngine;
using System.Collections;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

public class Ship : MonoBehaviour {

	private const float MAX_YAW = Mathf.PI/4;
	private const float MAX_PITCH = Mathf.PI/4;

    public GameObject myo = null;

    public Vector3 maxPos = new Vector3(10f, 5.5f, 0f);

    public float maxVelocity = 80f;

    Vector3 pos = new Vector3();
    float rotation = 0f;

    Pose baseLocation;

	//Myo math variables
	private float _myoAntiYaw = 0.0f;
	private float _myoReferenceRoll = 0.0f;
    
    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("r")) {
			recenterShipPosition();
		}

		pos = calculateShipPosition ();
		transform.position = pos;
	}

	Vector3 calculateShipPosition() {
		// Current zero roll vector and roll value.
//		Vector3 zeroRoll = computeZeroRollVector (myo.transform.forward);
//		float absoluteRoll = rollFromZero (zeroRoll, myo.transform.forward, myo.transform.up);
//		float roll = normalizeAngle (absoluteRoll - _myoReferenceRoll);

		Vector3 eulerAngles = myo.transform.eulerAngles;
		print (eulerAngles);
		float pitch = eulerAngles[1];
		float yaw = eulerAngles[2] - _myoAntiYaw;

		float x = maxPos [0] * yaw / MAX_YAW;
		float y = maxPos [1] * pitch / MAX_PITCH;
		//print(string.Format("x and y: {0}, {1} (p={2}, y={3})", x, y, pitch, yaw));
		return new Vector3 (x, y, 0.0f);
	}

	void recenterShipPosition() {
		Vector3 eulerAngles = myo.transform.eulerAngles;
		_myoReferenceRoll = eulerAngles [0];
		_myoAntiYaw = eulerAngles [2];
	}

	//Myo math functions

	// Compute the angle of rotation clockwise about the forward axis relative to the provided zero roll direction.
	// As the armband is rotated about the forward axis this value will change, regardless of which way the
	// forward vector of the Myo is pointing. The returned value will be between -180 and 180 degrees.
	float rollFromZero (Vector3 zeroRoll, Vector3 forward, Vector3 up)
	{
		// The cosine of the angle between the up vector and the zero roll vector. Since both are
		// orthogonal to the forward vector, this tells us how far the Myo has been turned around the
		// forward axis relative to the zero roll vector, but we need to determine separately whether the
		// Myo has been rolled clockwise or counterclockwise.
		float cosine = Vector3.Dot (up, zeroRoll);
		
		// To determine the sign of the roll, we take the cross product of the up vector and the zero
		// roll vector. This cross product will either be the same or opposite direction as the forward
		// vector depending on whether up is clockwise or counter-clockwise from zero roll.
		// Thus the sign of the dot product of forward and it yields the sign of our roll value.
		Vector3 cp = Vector3.Cross (up, zeroRoll);
		float directionCosine = Vector3.Dot (forward, cp);
		float sign = directionCosine < 0.0f ? 1.0f : -1.0f;
		
		// Return the angle of roll (in degrees) from the cosine and the sign.
		return sign * Mathf.Rad2Deg * Mathf.Acos (cosine);
	}
	
	// Compute a vector that points perpendicular to the forward direction,
	// minimizing angular distance from world up (positive Y axis).
	// This represents the direction of no rotation about its forward axis.
	Vector3 computeZeroRollVector (Vector3 forward)
	{
		Vector3 antigravity = Vector3.up;
		Vector3 m = Vector3.Cross (myo.transform.forward, antigravity);
		Vector3 roll = Vector3.Cross (m, myo.transform.forward);
		
		return roll.normalized;
	}

	// Adjust the provided angle to be within a -180 to 180.
	float normalizeAngle (float angle)
	{
		if (angle > 180.0f) {
			return angle - 360.0f;
		}
		if (angle < -180.0f) {
			return angle + 360.0f;
		}
		return angle;
	}

}
