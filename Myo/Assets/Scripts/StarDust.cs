using UnityEngine;
using System.Collections;

public class StarDust : MonoBehaviour {
	Vector3 DRIFVEL = new Vector3(0, 0, -0.1f);

	

	public Mesh CUBE = null,SPERE = null,TRIANGLE = null;


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


	private DustTypes dustType = DustTypes.CUBE;

	 public DustTypes DustType {
		get{return dustType;}
		set{
			dustType = value;
			this.GetComponent<MeshFilter>().mesh = findMesh(dustType);
			this.GetComponent<MeshRenderer>().enabled = true;
			
		}
	}

	public Vector3 vel = new Vector3();
	
	

	public void startDrift(){
		vel = DRIFVEL;
	}

	// Update is called once per frame
	void Update () {
		this.transform.position = this.transform.position + vel;
	}

	void OnTriggerEnter(Collider otherObj) {

		if (otherObj.gameObject.name == "DeathWall") {
			Destroy(this.gameObject);
		}
	}
}
