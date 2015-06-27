using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

public class Ship : MonoBehaviour {

	private readonly float MAX_YAW = 45.0f;
	private readonly float MAX_PITCH = 45.0f;
	private readonly float MAX_VELOCITY = 3.0f;
	private readonly Vector3 MAX_POS = new Vector3 (10f, 5.5f, 0);

	private readonly float HOLD_TIME_LENIENCY = 1.0f;

    public GameObject myo = null;
	public GameObject opposingShip = null;
	public GameObject starDustPrefab = null;

	private List<Rigidbody> collidingObjects = new List<Rigidbody>();
	private StarDust heldStarDust = null;
	private float holdRequestInitateTime;
	private float holdStartTime;
	private bool holdRequestIsActive;

    Vector3 pos = new Vector3();
    float rotation = 0f;
	private Pose _lastPose = Pose.Rest;

	//Myo math variables
	private float _myoAntiYaw = 0.0f;
	private float _myoReferenceRoll = 0.0f;
    
    // Use this for initialization
    void Start () {
		ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo> ();
		_lastPose = thalmicMyo.pose;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("r")) {
			recenterShipPosition();
		}

		Vector3 targetPos = calculateShipPosition ();
		Vector3 velocity = targetPos - transform.position;
		if (velocity.magnitude > MAX_VELOCITY) {
			velocity = velocity * velocity.magnitude / MAX_VELOCITY;
		}
		GetComponent<Rigidbody> ().AddForce (velocity, ForceMode.VelocityChange);


		float roll = calculateShipRoll ();
		transform.rotation = Quaternion.AngleAxis (roll, new Vector3 (0, 0, 1));

		//check for pose changes
		ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo> ();
		if (thalmicMyo != null && _lastPose != thalmicMyo.pose) {
			_lastPose = thalmicMyo.pose;

			GameObject dust;
			switch(_lastPose) {
			case Pose.Fist:
				print ("fist boys");
				holdRequestInitateTime = Time.realtimeSinceStartup;
				grabNearestCollidingObject();
				break;
			case Pose.WaveIn:
				print ("wave in boys");
				dust = (GameObject)Instantiate(starDustPrefab, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
				dust.GetComponent<StarDust>().DustType = DustTypes.SPERE;
//				dust = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//				dust.transform.position = transform.position;
//				dust.transform.rotation = transform.rotation;
//				Vector3 throwDir = new Vector3(-Mathf.Cos(roll*Mathf.PI/180), -Mathf.Sin (roll*Mathf.PI/180), 0);
//				float throwVelocity = 50.0f;
//				dust.AddComponent<Rigidbody>().AddForce(throwDir * throwVelocity, ForceMode.VelocityChange);
				break;
			case Pose.WaveOut:
				print ("wave out boys");
				dust = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				dust.transform.position = new Vector3(transform.position.x, transform.position.y, 2);
				dust.transform.rotation = transform.rotation;
//				Vector3 throwDir = new Vector3(Mathf.Cos(roll*Mathf.PI/180), Mathf.Sin (roll*Mathf.PI/180), 0);
//				float throwVelocity = 50.0f;
				Vector3 throwDir = opposingShip.transform.position - transform.position;
				float throwVelocity = 5.0f;
				dust.AddComponent<Rigidbody>().AddForce(throwDir * throwVelocity, ForceMode.VelocityChange);
				break;
			case Pose.DoubleTap:
				recenterShipPosition();
				break;
			default:
				break;
			}
		}
	}

	void grabNearestCollidingObject() {
		if (collidingObjects.Count > 0) {
			Rigidbody body = collidingObjects[0];
			float minDistSq = Vector3.SqrMagnitude(body.position - transform.position);
			int minDistIndex = 0;
			for (int i = 1; i < collidingObjects.Count; i++) {
				float distSq = Vector3.SqrMagnitude(body.position - transform.position);
				if (distSq < minDistSq) {
					minDistSq = distSq;
					minDistIndex = i;
				}
			}

			Rigidbody heldBody = collidingObjects[minDistIndex];
			print ("grabbed some body");
		}
	}

	// events

	void OnTriggerEnter(Collider otherObj) {
		print (string.Format ("{0} enters", otherObj.name));
//		if (otherObj.tag == "StarDust") {
			collidingObjects.Add(otherObj.attachedRigidbody);
//		}
		if (Time.realtimeSinceStartup - holdRequestInitateTime < HOLD_TIME_LENIENCY) {
			print ("grabbed star dust");
			heldStarDust = null;
		}
	}

	void OnTriggerExit(Collider otherObj) {
		print (string.Format ("{0} exits", otherObj.name));
//		if (otherObj.tag == "StarDust") {
		collidingObjects.Remove (otherObj.attachedRigidbody);
//		}
	}

	// calculations

	Vector3 calculateShipPosition() {

		//Euler angles are given in PYR (??)
		Vector3 eulerAngles = myo.transform.eulerAngles;
		float pitch = normalizeAngle(eulerAngles[0]);
		float yaw = normalizeAngle(eulerAngles[1] - _myoAntiYaw);

		float x = MAX_POS [0] * yaw / MAX_YAW;
		x = clampValue (x, -MAX_POS [0], MAX_POS [0]);
		
		float y = MAX_POS [1] * pitch / MAX_PITCH;
		y = clampValue (y, -MAX_POS [1], MAX_POS [1]);
//		print(string.Format("x and y: {0}, {1} (p={2}, y={3})", x, y, pitch, yaw));
		return new Vector3 (x, y, 0.0f);
	}

	float calculateShipRoll() {
		// Current zero roll vector and roll value.
		Vector3 zeroRoll = computeZeroRollVector (myo.transform.forward);
		float absRoll = rollFromZero (zeroRoll, myo.transform.forward, myo.transform.up);
		return absRoll - _myoReferenceRoll;
	}
	

	void recenterShipPosition() {
		Vector3 zeroRoll = computeZeroRollVector (myo.transform.forward);
		_myoReferenceRoll = rollFromZero (zeroRoll, myo.transform.forward, myo.transform.up);

		Vector3 eulerAngles = myo.transform.eulerAngles;
		_myoAntiYaw = eulerAngles [1];
	}

	//Myo math functions


	float clampValue(float val, float min, float max) {
		if (val > max) {
			return max;
		} 
		if (val < min) {
			return min;
		}
		return val;
	}
		
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
