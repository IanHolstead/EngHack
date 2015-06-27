using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class Nemesis : MonoBehaviour {

	public class Plan{
		public int dif;
		public Dust[] dusts;
	}

	public class Dust{
		public Vector3 pos;
		public bool key;
	}

	public delegate void SectionCleared(GameObject sender);
	public event SectionCleared Cleared;

	public List<GameObject> dust = new List<GameObject>();
	public List<GameObject> passWalls = new List<GameObject>();
	public List<Plan> plans = new List<Plan> ();

	public GameObject startDust = null;
    public GameObject passWall;

    public bool endOnFailTest = true;
    public Vector3 velocity = new Vector3 (0, 0, -5);



	public void createLevel(int dif){

		float zset = 100;
		for (int i = 0; i <3; ++i) {
			createSector(ref zset, dif);
		}

	}

	public void createSector(ref float zset, int dif){
		DustTypes pass = (DustTypes)Random.Range(0,3);
		float start = zset;
		DustTypes[] failing = failingTypes (pass);

		if (plans.Count > 0) {
			for (int i = 0; i <10; ++i) {
				createPlan(new DustTypes[]{pass}, failing, ref zset, plans[Random.Range(0,plans.Count)]);
			}

		}

		for (int i = 0; i <5; ++i) {
			spawnDust(pass,new Vector3(Random.Range(-10.0f,10.0f),
			                           Random.Range(-5.0f,5.0f),
			                           Random.Range(start,zset)));
		}

		for (int i = 0; i <40; ++i) {
			spawnDust(failing[Random.Range(0,failing.Length)],new Vector3(Random.Range(-10.0f,10.0f),
			                                                   				Random.Range(-5.0f,5.0f),
			                                                   				Random.Range(start,zset)));
		}



		zset += 20;
		
		print (zset);
		SpawnWall (pass, zset);

		zset += 30;
	}

	private DustTypes[] failingTypes(DustTypes passingType){
		switch (passingType) {
		case DustTypes.CUBE: return new DustTypes[]{DustTypes.SPERE,DustTypes.TRIANGLE};
		case DustTypes.SPERE: return new DustTypes[]{DustTypes.CUBE,DustTypes.TRIANGLE};
		case DustTypes.TRIANGLE: return new DustTypes[]{DustTypes.SPERE,DustTypes.CUBE};

		}
		return new DustTypes[]{};

	}

	private void createPlan(DustTypes[] pass, DustTypes[] fail, ref float zset, Plan plan){
		Vector3 dpos = new Vector3 (0, 0, zset);
		float max = 0;
		foreach (Dust dust in plan.dusts) {
			addTypedDust(dust.key,dust.pos + dpos, pass, fail);
			if(dust.pos.z > max)
				max = dust.pos.z;
		}

		max += 20;

		zset += max;
	}

	private void addTypedDust(bool key, Vector3 pos, DustTypes[] pass, DustTypes[] fail){
		if (key && (pass.Length > 0)) {
			spawnDust(pass[Random.Range(0,pass.Length)],pos);
		} else if (!key && (fail.Length > 0)) {

			spawnDust(fail[Random.Range(0,fail.Length)],pos);
		}

	}

	public void loadPlans(){
		XmlDocument doc = new XmlDocument ();
		using (FileStream file = new FileStream(Application.dataPath + "/plans.xml",FileMode.Open)) {
			doc.Load (file);
		}
	

		foreach (XmlNode node in doc.DocumentElement.ChildNodes) {
			if(node.Name == "plan"){
				Plan plan = new Plan();
				plan.dif = getDificulty(node.Attributes);
				List<Dust> dusts = new List<Dust>();
				foreach(XmlNode subNode in node.ChildNodes){
					if(subNode.Name == "dust"){
						Dust dust = new Dust();
						dust.key = getDustType(subNode.Attributes);
						dust.pos = getPos(subNode.InnerText);
						dusts.Add(dust);
					}
				}
				plan.dusts = dusts.ToArray();
				plans.Add(plan);
			}
		}


	}

	private Vector3 getPos(string inter){
		string[] vals = inter.Split (',');

		if (vals.Length != 3)
			return new Vector3 ();

		return new Vector3 (int.Parse (vals[0]), int.Parse (vals[1]), int.Parse (vals[2]));
	}

	private bool getDustType(XmlAttributeCollection col){
		string dif = findAtrib (col, "type");
		
		if (dif == "") {
			return false;
		} else if(dif == "key") {
			return true;
		}

		return false;
	}

	private int getDificulty(XmlAttributeCollection col){
		string dif = findAtrib (col, "dif");

		if (dif == "") {
			return 0;
		} else {
			return int.Parse (dif);
		}
	}

	private string findAtrib(XmlAttributeCollection col, string name){
		foreach (XmlAttribute atrib in col) {
			if (atrib.Name == name) {
				return atrib.Value;
			}
		}
		return "";
	}


	public void spawnDust(DustTypes type,Vector3 pos){
		GameObject newDust = (GameObject)Instantiate(startDust, pos, Quaternion.identity);
		
		StarDust script = newDust.GetComponent<StarDust> ();
		script.DustType = type;
		script.Velocity = velocity;
		script.spin ();

	}

	// Use this for initialization
	void Start () {
		loadPlans ();
	}

	void SpawnWall(DustTypes passWallType, float zset)
    {
		GameObject passWallInstance = (GameObject)Instantiate(passWall, new Vector3(0, 0, zset), Quaternion.AngleAxis(-90, new Vector3(1,0,0)));
		passWalls.Add (passWallInstance);
		PassWall script = passWallInstance.GetComponent<PassWall> ();
		script.Setup(passWallType, endOnFailTest, velocity.z);
		script.Removed += new PassWall.WallRemoved (wallRemoved);

    }

	void wallRemoved(GameObject sender){
		passWalls.Remove (sender);

		if (Cleared != null)
			Cleared(this.gameObject);
	}
}
