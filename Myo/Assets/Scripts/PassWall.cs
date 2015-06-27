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
        pos.z += velocity;
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
        bool player1Dead = false;
        bool player2Dead = false;
        foreach (Ship ship in FindObjectsOfType(typeof(Ship)))
        {
            if (ship.GetDustType() != type)
            {
                if (ship.gameObject.name == "Player1")
                {
                    player1Dead = true;
                }
                else
                {
                    player2Dead = true;
                }
            }
        }
        if (player1Dead)
        {
            if (player2Dead)
            {
                print("You Both Died! How could you!?!");
            }
            else
            {
                print("Player 2 Wins!");
            }
        }
        else
        {
            print("Player 1 Wins!");
        }
        if (endGame)
        {
            Application.Quit();
        }
    }

    public void Setup(DustTypes dustType, bool leathal, float velocity)
    {
        type = dustType;
        endGame = leathal;
        this.velocity = velocity;
    }
}
