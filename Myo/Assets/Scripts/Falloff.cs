using UnityEngine;
using System.Collections;

public class Falloff : MonoBehaviour {

    GameObject PlayerCamera;
    public float invisibleUntilDistance = 200f;
    public float visibleStartDistance = 100f;
    public float visibleUntilDistance = 0f;
    public float invisibleStartDistance = -10f;

	// Use this for initialization
	void Start () {
        if (visibleStartDistance > invisibleUntilDistance)
        {
            invisibleUntilDistance = visibleStartDistance + 100;
        }
        PlayerCamera = GameObject.FindGameObjectWithTag("MainCamera");
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
        if (distanceToCamera <= invisibleStartDistance)
        {
            return 0f;
        }
        else if (distanceToCamera <= visibleUntilDistance)
        {
            return (distanceToCamera - invisibleStartDistance) / (visibleUntilDistance - invisibleStartDistance);
        }
        else if (distanceToCamera <= visibleStartDistance)
        {
            return 1f;
        }
        else if(distanceToCamera <= invisibleUntilDistance)
        {
            return 1f - ((distanceToCamera - visibleStartDistance) / (invisibleUntilDistance - visibleStartDistance));
        }
        return 0f;
    }
}
