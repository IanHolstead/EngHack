using UnityEngine;
using System.Collections.Generic;

public class PassWall : MonoBehaviour {

    public float velocity = 0.1f;
    public DustTypes type;
    public Material cubeMat, sphereMat, triangleMat;

    // Use this for initialization
    void Start () {
        //this.GetComponent<MeshRenderer>().material = findMatterial(type);
        this.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", findMatterial(type));
    }

    public Color findMatterial(DustTypes dustType)
    {
        switch (dustType)
        {
            case DustTypes.CUBE:
                return Color.blue;
            case DustTypes.SPERE:
                return Color.green;
            case DustTypes.TRIANGLE:
                return Color.magenta;
        }
        return Color.black;
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
