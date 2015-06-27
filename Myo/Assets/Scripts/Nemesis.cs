using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Nemesis : MonoBehaviour {

	private LinkedList<GameObject> dust = new LinkedList<GameObject>();
	private LinkedList<GameObject> passWalls = new LinkedList<GameObject>();

	public GameObject startDust = null;


	public void createLevel(){

		//GameObject newDust = Instantiate<GameObject> (startDust);

		GameObject newDust = (GameObject)Instantiate(startDust,new Vector3 (5, 0, 0),Quaternion.identity);

		StarDust script = newDust.GetComponent<StarDust> ();
		script.DustType = DustTypes.SPERE;
		script.startDrift ();
		Debug.Log ("spawn");

	}

	// Use this for initialization
	void Start () {
		createLevel ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
