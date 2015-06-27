using UnityEngine;
using System.Collections;

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
        foreach (GameObject ship in GameObject.FindGameObjectsWithTag("Player"))
        {
            //if ((Ship)ship.type != type) {YOU DONE};
        }
    }

    void SetType(DustTypes dustType)
    {
        type = dustType;
    }
}
