using UnityEngine;
using System.Collections;

public class FakeDust : MonoBehaviour {

	public Mesh Cube, Spere, Triangle;
	const float ZSET = 100;
	public Vector3 vel = new Vector3(0,0,-20), rotVel = new Vector3();

	// Use this for initialization
	void Start () {
		spawn ();
	}

	public void spawn(){
		this.transform.position = spawnPos ();
		this.GetComponent<MeshFilter> ().mesh = spawnMesh ();
		spin ();

	}

	public Mesh spawnMesh(){
		switch (Random.Range (0, 3)) {
		case 0: return Cube;
		case 1: return Spere;
		case 2: return Triangle;
		}
		return Cube;
	}

	 public void OnBecameInvisible () {
		spawn ();
	}

	public Vector3 spawnPos(){
		switch (Random.Range (0, 8)) {
		case 0: return new Vector3(Random.Range (15.0f, 50.0f),Random.Range (7.0f, 30.0f),ZSET);
		case 1: return new Vector3(Random.Range (-15.0f, 15.0f),Random.Range (7.0f, 30.0f),ZSET);
		case 2: return new Vector3(Random.Range (-50.0f, -15.0f),Random.Range (7.0f, 30.0f),ZSET);
		case 3: return new Vector3(Random.Range (-50.0f, -15.0f),Random.Range (-7.0f, 7.0f),ZSET);
		case 4: return new Vector3(Random.Range (-50.0f, -15.0f),Random.Range (-30.0f, 7.0f),ZSET);
		case 5: return new Vector3(Random.Range (-15.0f, 15.0f),Random.Range (-30.0f, 7.0f),ZSET);
		case 6: return new Vector3(Random.Range (15.0f, 50.0f),Random.Range (-30.0f, 7.0f),ZSET);
		case 7: return new Vector3(Random.Range (15.0f, 50.0f),Random.Range (-7.0f, 7.0f),ZSET);
		}

		return new Vector3 ();

	}

	public void spin(){
		rotVel = new Vector3(Random.Range(10f,90f),Random.Range(10f,90f),Random.Range(10f,90f));
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position += vel * Time.deltaTime;
		this.transform.Rotate (rotVel * Time.deltaTime);
	}
}
