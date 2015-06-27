using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Nemesis : MonoBehaviour {

	private LinkedList<GameObject> dust = new LinkedList<GameObject>();
	private LinkedList<GameObject> passWalls = new LinkedList<GameObject>();

	public GameObject startDust = null;


	public void createLevel(){

		GameObject newDust = GameObject.Instantiate<GameObject> (startDust);

		newDust.transform.position = new Vector3 (0, 0, 100);

		StarDust script = newDust.GetComponent<StarDust> ();
		script.DustType = DustTypes.CUBE;
		script.startDrift ();

	}

	// Use this for initialization
	void Start () {
		createLevel ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
