using UnityEngine;
using System.Collections;

public class Falloff : MonoBehaviour {

    public GameObject PlayerCamera;
    public float invisibleDistance = 200f;
    public float visibleDistance = 100f;

	// Use this for initialization
	void Start () {
        if (visibleDistance < invisibleDistance)
        {
            invisibleDistance = visibleDistance + 100;
        }
	}
	
	// Update is called once per frame
	void Update () {
        Color colour = GetComponent<MeshRenderer>().material.color;
        colour.a = getTransperency();
        GetComponent<MeshRenderer>().material.color = colour;
    }

    private float getTransperency ()
    {
        float distanceToCamera = Mathf.Abs(PlayerCamera.transform.position.z - transform.position.z);
        if (distanceToCamera <= 100f)
        {
            print("1");
            return 1f;
        }
        else if(distanceToCamera <= 200)
        {
            print(1f - ((distanceToCamera - visibleDistance) / 100));
            return 1f - ((distanceToCamera - visibleDistance) / 100);
        }
        print("0");
        return 0f;
    }
}
