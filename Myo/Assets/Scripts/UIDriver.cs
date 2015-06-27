using UnityEngine;
using System.Collections.Generic;

public class UIDriver : MonoBehaviour {
    private DustTypes prevPassWall = DustTypes.CUBE;
    private int difficulty = 1;
    private GameState state;
    private bool endGame = true;

    public enum GameState
    {
        START = 0,
        PLAYING = 1,
        END = 2
    }

    public GameState State
    {
        get { return state; }
        set
        {
            state = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        state = GameState.START;
        showStateText("Punch The Screen\nTo Begin");
        showHUD(false);
        showGateWarning(false);

        GetComponent<Nemesis>().Cleared += new Nemesis.SectionCleared(updatePassWall);
        GetComponent<Nemesis>().Finnished += new Nemesis.LevelFinnished(startLevel);
        //GetComponent<PassWall>().Removed += new PassWall.WallRemoved(updatePassWall);
    }

    private void updatePassWall(GameObject obj)
    {
        if (obj.GetComponent<Nemesis>().passWalls.Count > 0)
        {
            prevPassWall = obj.GetComponent<Nemesis>().passWalls[0].GetComponent<PassWall>().type;
            showGateWarning(true);
        }
        else
        {
            showGateWarning(false);
        }
    }

    private void startLevel(GameObject obj)
    {
        gameObject.GetComponent<Nemesis>().removeAll();
        state = GameState.PLAYING;
        GetComponent<Nemesis>().createLevel(difficulty);
        updatePassWall(gameObject);
        difficulty++;
    }

    private void resetLevel()
    {
        state = GameState.END;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<Ship>().Life = 3;
        }

        difficulty = 1;

        //TODO: set this to 0       gameObject.GetComponent<Nemesis>().velocity = 
    }

    public void showStateText(string text)
    {
        GameObject.Find("State text").GetComponent<TextMesh>().text = text;
    }

    public void showHUD(bool visible)
    {
        if (visible)
        {
            if (state != GameState.PLAYING)
            {
                startLevel(gameObject);
            }
            GameObject.Find("Player 1 life text").GetComponent<TextMesh>().text = "P1 Life: " + GameObject.Find("Player1").GetComponent<Ship> ().Life.ToString();
            GameObject.Find("Player 2 life text").GetComponent<TextMesh>().text = "P2 Life: " + GameObject.Find("Player2").GetComponent<Ship>().Life.ToString();
            GameObject.Find("Gate text").GetComponent<TextMesh>().text = "Next Gate:";
        }
        else
        {
            if (state == GameState.PLAYING)
            {
                resetLevel();
            }
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

    public void showGateWarning(bool visible)
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
            switch (prevPassWall)
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

   // public void showEndText(bool p1Death)
    //{
        //state = GameState.END;
       // if (p1Death) {
        //    GameObject.Find("State text").GetComponent<TextMesh>().text = "Player 1 Has Died\nPunch The Screen\nTo Play Again";
        //} else {
       //     GameObject.Find("State text").GetComponent<TextMesh>().text = "Player 2 Has Died\nPunch The Screen\nTo Play Again";
       // }
    //}

    public void setup(bool lethal)
    {
        endGame = lethal;
    }
}
