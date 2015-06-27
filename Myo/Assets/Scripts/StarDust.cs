using UnityEngine;
using System.Collections;

public class StarDust : MonoBehaviour {
	const double DRIFSPEED = -1;


	public enum DustType{
		Cube = 0,
		Sphere = 1,
		Triangle = 2
	}

	private DustType type;

	private DustType Type {
		get{return type;}
		set{
			type = value;
			MeshRenderer render = pos.GetChild (this.type).GetComponents<MeshRenderer> ();
			render.enabled = true;
		}
	}

	public double speed = 0;

	private Transform pos = GetComponent<Transform>();
	
	public void startDrift(){
		speed = DRIFSPEED;
	}

	// Update is called once per frame
	void Update () {
		pos.forward += speed;
	}

	void OnCollisionEnter(otherObj: Collision) {
		if (otherObj.tag == "Arrow") {
			ApplyDamage(10);
		}
	}
}
