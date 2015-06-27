using UnityEngine;
using System.Collections;

public class StarDust : MonoBehaviour {
	
	public Mesh CUBE = null, SPERE = null, TRIANGLE = null;

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
	
	public Vector3 Velocity{
		get{return vel;}
		set{vel = value;}
	}

	public Vector3 vel = new Vector3();
	public Vector3 rotVel = new Vector3();

	public void spin(){
		rotVel = new Vector3(Random.Range(10f,90f),Random.Range(10f,90f),Random.Range(10f,90f));
	}

	// Update is called once per frame
	void Update () {
		this.transform.position += vel * Time.deltaTime;
		this.transform.Rotate(rotVel * Time.deltaTime);
	}

	void OnTriggerEnter(Collider otherObj) {

		if (otherObj.gameObject.name == "DeathWall") {
			Destroy(this.gameObject);
		}
	}
}
