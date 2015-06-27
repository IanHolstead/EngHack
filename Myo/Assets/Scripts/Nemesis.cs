using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Nemesis : MonoBehaviour {

	private LinkedList<GameObject> dust = new LinkedList<GameObject>();
	private LinkedList<GameObject> passWalls = new LinkedList<GameObject>();

	public GameObject startDust = null;
    public GameObject passWall;

    public bool endOnFailTest = true;
    public float velocity = -0.1f;

    DustTypes nextPassWallType;
    public int wallSpawnTime = 15;
    float timeSinceWall = 0f;

	public void createLevel(){

		//GameObject newDust = Instantiate<GameObject> (startDust);

		GameObject newDust = (GameObject)Instantiate(startDust,new Vector3 (5, 0, 20),Quaternion.identity);

		StarDust script = newDust.GetComponent<StarDust> ();
        script.SetVelocity(velocity);
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
        if (passWalls.Count != 0)
        {
            timeSinceWall = 0f;
        }
        else
        {
            timeSinceWall += Time.deltaTime;
            if (timeSinceWall > wallSpawnTime)
            {
                
            }
        }
        
	}

    void SpawnWall()
    {
        GameObject passWallInstance = (GameObject)Instantiate(passWall, new Vector3(0, 0, 200), transform.rotation);
        passWallInstance.GetComponent<PassWall>().Setup(nextPassWallType, endOnFailTest, velocity);
    }
}
