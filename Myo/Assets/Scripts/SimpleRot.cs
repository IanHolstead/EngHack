using UnityEngine;
using System.Collections;

public class SimpleRot : MonoBehaviour {
	Vector3 rotVel;

	// Use this for initialization
	void Start () {
		rotVel = new Vector3(Random.Range(10f,90f),Random.Range(10f,90f),Random.Range(10f,90f));
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.Rotate (rotVel * Time.deltaTime);
	}
}
