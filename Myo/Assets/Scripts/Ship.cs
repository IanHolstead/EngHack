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
	private readonly float MAX_VELOCITY = 80.0f;
	private readonly float MAX_ACCEL = 400.0f;
	private readonly Vector3 MAX_POS = new Vector3 (10f, 5.5f, 0);
    
    private DustTypes type = DustTypes.CUBE;
    private int life = 3;

	private readonly float HOLD_TIME_LENIENCY = 2.0f;
	private readonly float HOLD_DISTANCE = 1.0f;
	private readonly float HOLD_VELOCITY_FACTOR = 0.2f;
	private readonly float THROW_VELOCITY = 10.0f;

    public GameObject myo = null;
	public GameObject opposingShip = null;

    private List<Rigidbody> collidingObjects = new List<Rigidbody>();
	private Rigidbody heldObject = null;
	private float holdRequestInitateTime;
	private float holdStartTime;
	private bool holdRequestIsActive;

    public DustTypes GetDustType()
    {
        return type;
    }

    public void setType(DustTypes type) {
        this.type = type;
    }

    public int Life
    {
        get { return life; }
        set
        {
            life = value;
        }
    }

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
//		Vector3 accelTick = targetPos - transform.position;
//		if (accelTick.magnitude > MAX_ACCEL * Time.deltaTime) {
//			accelTick = accelTick * accelTick.magnitude / (MAX_ACCEL * Time.deltaTime);
//		}
//		GetComponent<Rigidbody> ().AddForce (accelTick, ForceMode.VelocityChange);
//		Vector3 finalVelocity = GetComponent<Rigidbody> ().velocity;
//		if (finalVelocity.sqrMagnitude > MAX_VELOCITY * MAX_VELOCITY) {
//			finalVelocity = finalVelocity / finalVelocity.magnitude * MAX_VELOCITY;
//		}
		Vector3 dist = targetPos - transform.position;
		Vector3 finalVelocity;
		float velocityFactor = (heldObject == null)? 1.0f : HOLD_VELOCITY_FACTOR;
		velocityFactor *= (1 - Mathf.Exp(-dist.magnitude));
		GetComponent<Rigidbody>().velocity = dist / dist.magnitude * MAX_VELOCITY * velocityFactor;

		float roll = calculateShipRoll ();
		transform.rotation = Quaternion.AngleAxis (roll, new Vector3 (0, 0, 1));

		updateHeldObject ();
		

		//check for pose changes
		ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo> ();
		if (thalmicMyo != null && _lastPose != thalmicMyo.pose) {
			_lastPose = thalmicMyo.pose;

			GameObject dust;
			switch(_lastPose) {
			case Pose.Fist:
				print ("fist boys");
				throwHeldObject ();
				holdRequestInitateTime = Time.realtimeSinceStartup;
				break;
			case Pose.WaveIn:
			case Pose.WaveOut:
				throwHeldObject();
				break;
			case Pose.DoubleTap:
				recenterShipPosition();
				break;
			default:
				break;
			}
		}

	}


	void throwHeldObject() {
		if (heldObject != null) {
			Vector3 direction = heldObject.transform.position - transform.position;
			heldObject.GetComponent<Rigidbody>().velocity = direction / direction.magnitude * THROW_VELOCITY;
			heldObject = null;
		}
	}

	void updateHeldObject() {
		if (heldObject != null) {
			Vector3 direction = opposingShip.transform.position - transform.position;
			Vector3 offset = direction / direction.magnitude * HOLD_DISTANCE;
			heldObject.GetComponent<Rigidbody> ().position = transform.position + offset;
		}
	}

	// events

	void OnTriggerEnter(Collider otherObj) {
		if (otherObj.gameObject.tag == "Dust") {
			collidingObjects.Add(otherObj.attachedRigidbody);
			if (heldObject == null && 
			   		Time.realtimeSinceStartup - holdRequestInitateTime < HOLD_TIME_LENIENCY) {
				print ("grabbed star dust");
				heldObject = otherObj.attachedRigidbody;
			} else {
				this.type = otherObj.gameObject.GetComponent<StarDust>().DustType;
				this.GetComponent<MeshFilter>().mesh = otherObj.gameObject.GetComponent<StarDust>().findMesh(type);
				Destroy (otherObj.gameObject);
			}
        }
        else if (otherObj.gameObject.tag == "PassWall")
        {
            if (type != otherObj.GetComponent<PassWall>().type)
            {
                life--;
                if (life <= 0)
                {
                    if ((GameObject.Find("Player1").GetComponent<Ship>().Life <= 0))
                    {
                        GameObject.Find("Spawner").GetComponent<UIDriver>().showEndText(true);
                    }
                    else
                    {
                        GameObject.Find("Spawner").GetComponent<UIDriver>().showEndText(false);
                    }
                    GameObject.Find("Spawner").GetComponent<UIDriver>().showHUD(false);
                }
                else
                {
                    GameObject.Find("Spawner").GetComponent<UIDriver>().showHUD(true);
                }
            }
        }
	}

	void OnTriggerExit(Collider otherObj) {
		if (otherObj.gameObject.tag == "Dust") {
			collidingObjects.Remove (otherObj.attachedRigidbody);
		}
	}

	Vector3 calculateShipPosition() {

		//Euler angles are given in PYR (??)
		Vector3 eulerAngles = myo.transform.eulerAngles;
		float pitch = normalizeAngle(eulerAngles[0]);
		float yaw = normalizeAngle(eulerAngles[1] - _myoAntiYaw);

		float x = MAX_POS [0] * yaw / MAX_YAW;
		x = clampValue (x, -MAX_POS [0], MAX_POS [0]);
		
		float y = -MAX_POS [1] * pitch / MAX_PITCH;
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
        GameObject.Find("Spawner").GetComponent<UIDriver> ().clearStateText();
        GameObject.Find("Spawner").GetComponent<UIDriver> ().showHUD(true);
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
