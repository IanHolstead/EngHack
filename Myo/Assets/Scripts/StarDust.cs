using UnityEngine;
using System.Collections;

public class StarDust : MonoBehaviour {
	const float DRIFSPEED = -0.1f;

	

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

	void OnTriggerEnter(Collider otherObj) {
		Debug.Log ("co");

		if (otherObj.gameObject.name == "DeathWall") {
			Debug.Log ("death");
			Destroy(this.gameObject);
		}
	}
}
