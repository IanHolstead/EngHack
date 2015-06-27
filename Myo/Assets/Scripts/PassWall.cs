using UnityEngine;
using System.Collections.Generic;

public class PassWall : MonoBehaviour {

    public float velocity = 0.1f;
    public DustTypes type;
	
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;
		pos.z += velocity * Time.deltaTime;;
        transform.position = pos;
	}

	public delegate void WallRemoved(GameObject sender);
	public event WallRemoved Removed;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "DeathWall")
        {
			if (Removed != null)
				Removed(this.gameObject);


            Destroy(gameObject);
        }
    }

    public void Setup(DustTypes dustType, float velocity)
    {
        type = dustType;
        this.velocity = velocity;
    }
}
