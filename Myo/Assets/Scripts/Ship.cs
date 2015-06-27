using UnityEngine;
using System.Collections;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

public class Ship : MonoBehaviour {

    public GameObject myo = null;

    public Vector3 maxPos = new Vector3(10f, 5.5f, 0f);

    public float maxVelocity = 80f;

    Vector3 pos = new Vector3();
    float rotation = 0f;

    Pose baseLocation;
    
    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
