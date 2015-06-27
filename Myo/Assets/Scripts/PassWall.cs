using UnityEngine;
using System.Collections.Generic;

public class PassWall : MonoBehaviour {

    public float velocity = 0.1f;
    DustTypes type;
    bool endGame = true;
	
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;
        pos.z -= velocity;
        transform.position = pos;
	}

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            TestEndGame();
        }
    }

    void TestEndGame()
    {
        foreach (Ship ship in FindObjectsOfType(typeof(Ship)))
        {
            //if (ship.type != type)
            //{

            //}
        }
    }

    public void Setup(DustTypes dustType, bool leathal, float velocity)
    {
        type = dustType;
        endGame = leathal;
        this.velocity = velocity;
    }
}
