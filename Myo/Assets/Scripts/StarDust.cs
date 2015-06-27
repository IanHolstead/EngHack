using UnityEngine;
using System.Collections;

public class StarDust : MonoBehaviour {
	const float DRIFSPEED = -1;


	public enum DustType{
		Cube = 0,
		Sphere = 1,
		Triangle = 2
	}

	public Mesh CUBE = null,SPERE = null,TRIANGLE = null;


	public Mesh findMesh(DustType dustType){
		switch (dustType) {
		case DustType.Cube:
			return CUBE;
		case DustType.Sphere:
			return SPERE;
		case DustType.Triangle:
			return TRIANGLE;
		}
		return null;
	}


	private DustType dustType = DustType.Cube;

	private DustType Type {
		get{return dustType;}
		set{
			dustType = value;
			this.GetComponent<MeshFilter>().mesh = findMesh(dustType);
			this.GetComponent<MeshRenderer>().enabled = true;
			
		}
	}

	public float speed = 0;
	
	

	public void startDrift(){
		speed = DRIFSPEED;
	}

	// Update is called once per frame
	void Update () {
		Vector3 pos = this.transform.position;
		pos.z += speed;
		this.transform.position = pos;
	}

	void OnCollisionEnter(Collision otherObj) {

		if (otherObj.gameObject.name == "DeathWall") {
			Destroy(this.gameObject);
		}
	}
}
