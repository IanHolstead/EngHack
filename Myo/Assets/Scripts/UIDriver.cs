using UnityEngine;
using System.Collections.Generic;

public class UIDriver : MonoBehaviour {
    private DustTypes prevPassWall = DustTypes.CUBE;

    // Use this for initialization
    void Start()
    {
        showStartText();
        showHUD(false);
        showGateWarning(prevPassWall, false);
    }

    // Update is called once per frame
    void Update() {

        if (prevPassWall != GameObject.Find("Spawner").GetComponent<Nemesis>().passWalls[0].GetComponent<PassWall> ().type)
        {
            prevPassWall = GameObject.Find("Spawner").GetComponent<Nemesis>().passWalls[0].GetComponent<PassWall>().type;
            showGateWarning(prevPassWall, true);
        }

        if (GameObject.Find("Player1").GetComponent<Ship> ().Life <= 0)
        {
            showGateWarning(prevPassWall, false);
            showHUD(false);
            showEndText(true);
        }
        else if (GameObject.Find("Player2").GetComponent<Ship>().Life <= 0)
        {
            showGateWarning(prevPassWall, false);
            showHUD(false);
            showEndText(false);
        }
    }

    private void showStartText()
    {
        GameObject.Find("State text").GetComponent<TextMesh>().text = "Punch The Screen\nTo Begin";
    }

    public void showHUD(bool visible)
    {
        if (visible)
        {
            GameObject.Find("Player 1 life text").GetComponent<TextMesh>().text = "P1 Life: " + GameObject.Find("Player1").GetComponent<Ship> ().Life.ToString();
            GameObject.Find("Player 2 life text").GetComponent<TextMesh>().text = "P2 Life: " + GameObject.Find("Player2").GetComponent<Ship>().Life.ToString();
            GameObject.Find("Gate text").GetComponent<TextMesh>().text = "Next Gate:";
        }
        else
        {
            GameObject.Find("Player 1 life text").GetComponent<TextMesh>().text = "";
            GameObject.Find("Player 2 life text").GetComponent<TextMesh>().text = "";
            GameObject.Find("Gate text").GetComponent<TextMesh>().text = "";
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("HUD"))
        {
            Color colour = obj.GetComponent<Renderer>().material.color;
            if (visible)
            {
                colour.a = 1;
            }
            else
            {
                colour.a = 0;
            }

            obj.GetComponent<Renderer>().material.color = colour;
        }
    }

    private void showGateWarning(DustTypes type, bool visible)
    {
        Color colour = GameObject.Find("Gate Cube").GetComponent<Renderer>().material.color;
        colour.a = 0;
        GameObject.Find("Gate Cube").GetComponent<Renderer>().material.color = colour;

        colour = GameObject.Find("Gate Sphere").GetComponent<Renderer>().material.color;
        colour.a = 0;
        GameObject.Find("Gate Sphere").GetComponent<Renderer>().material.color = colour;

        colour = GameObject.Find("Gate Triangle").GetComponent<Renderer>().material.color;
        colour.a = 0;
        GameObject.Find("Gate Triangle").GetComponent<Renderer>().material.color = colour;

        if (visible)
        {
            switch (type)
            {
                case DustTypes.CUBE:
                    colour = GameObject.Find("Gate Cube").GetComponent<Renderer>().material.color;
                    colour.a = 1;
                    GameObject.Find("Gate Cube").GetComponent<Renderer>().material.color = colour;
                    break;
                case DustTypes.SPERE:
                    colour = GameObject.Find("Gate Sphere").GetComponent<Renderer>().material.color;
                    colour.a = 1;
                    GameObject.Find("Gate Sphere").GetComponent<Renderer>().material.color = colour;
                    break;
                case DustTypes.TRIANGLE:
                    colour = GameObject.Find("Gate Triangle").GetComponent<Renderer>().material.color;
                    colour.a = 1;
                    GameObject.Find("Gate Triangle").GetComponent<Renderer>().material.color = colour;
                    break;
            }
        }
    }

    private void showEndText(bool p1Death)
    {
        if (p1Death) {
            GameObject.Find("State text").GetComponent<TextMesh>().text = "Player 2 Has Died\nPunch The Screen\nTo Play Again";
        } else {
            GameObject.Find("State text").GetComponent<TextMesh>().text = "Player 1 Has Died\nPunch The Screen\nTo Play Again";
        }
    }

    public void clearStateText()
    {
        GameObject.Find("State text").GetComponent<TextMesh>().text = "";
    }
}
