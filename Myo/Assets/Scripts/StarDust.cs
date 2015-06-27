using UnityEngine;
using System.Collections;

public class StarDust : MonoBehaviour {
	
	public Mesh CUBE = null, SPERE = null, TRIANGLE = null;
    public Material cubeMat, sphereMat, triangleMat;

	public Mesh findMesh(DustTypes dustType){
		switch (dustType) {
		case DustTypes.CUBE:
			return CUBE;
		case DustTypes.SPERE:
			return SPERE;
		case DustTypes.TRIANGLE:
			return TRIANGLE;
		}
		return null;
	}
    
    public Material findMatterial(DustTypes dustType)
    {
        switch (dustType)
        {
            case DustTypes.CUBE:
                return cubeMat;
            case DustTypes.SPERE:
                return sphereMat;
            case DustTypes.TRIANGLE:
                return triangleMat;
        }
        return null;
    }
	public delegate void DustRemoved(GameObject sender);
	public event DustRemoved Removed;


    private DustTypes dustType = DustTypes.CUBE;

	 public DustTypes DustType {
		get{return dustType;}
		set{
			dustType = value;
			this.GetComponent<MeshFilter>().mesh = findMesh(dustType);
            this.GetComponent<MeshRenderer>().material = findMatterial(dustType);
			this.GetComponent<MeshRenderer>().enabled = true;
			
		}
	}
	
	public Vector3 Velocity{
		get{return vel;}
		set{
			vel = value;
			GetComponent<Rigidbody>().velocity = vel;
		}
	}

	public Vector3 vel = new Vector3();
	public Vector3 rotVel = new Vector3();

	public void spin(){
		GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(10f,90f),Random.Range(10f,90f),Random.Range(10f,90f));
	}

	// Update is called once per frame
	void Update () {
//		this.transform.position += vel * Time.deltaTime;
//		this.transform.Rotate(rotVel * Time.deltaTime);
	}

	void OnTriggerEnter(Collider otherObj) {

		if (otherObj.gameObject.tag == "DeathWall") {
			Destroy(this.gameObject);
			if (Removed != null)
				Removed(this.gameObject);
		}
	}
}
