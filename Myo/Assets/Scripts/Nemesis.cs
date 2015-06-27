using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class Nemesis : MonoBehaviour {

	private class Plan{
		public int dif;
		public Dust[] dusts;
	}

	private class Dust{
		public Vector3 pos;
		public bool key;
	}

	private List<GameObject> dust = new List<GameObject>();
	private List<GameObject> passWalls = new List<GameObject>();
	private List<Plan> plans = new List<Plan> ();

	public GameObject startDust = null;
    public GameObject passWall;

    public bool endOnFailTest = true;
    public Vector3 velocity = new Vector3 (0, 0, -5);

    DustTypes nextPassWallType = DustTypes.SPERE; //CHOSEN BY SPELLING MISTAKE
    public int wallSpawnTime = 15;
    public float wallSpawnLocationZ = 200f;
    float timeSinceWall = 0f;

	public void createLevel(int dif){

		spawnDust (DustTypes.CUBE, new Vector3 (0, 5, 20));
		spawnDust (DustTypes.CUBE, new Vector3 (0, -5, 20));

	}

	public void createSector(DustTypes[] pass, DustTypes[] fail, float zset){

	}

	private void createPlan(DustTypes[] pass, DustTypes[] fail, float zset, Plan plan){
		Vector3 dpos = new Vector3 (0, 0, zset);
		foreach (Dust dust in plan.dusts) {
			addTypedDust(dust.key,dust.pos + dpos, pass, fail);
		}
	}

	private void addTypedDust(bool key, Vector3 pos, DustTypes[] pass, DustTypes[] fail){
		if (key && (pass.Length > 0)) {
			spawnDust(pass[Mathf.RoundToInt(Random.Range(0,pass.Length - 1))],pos);
		} else if (!key && (fail.Length > 0)) {
			spawnDust(fail[Mathf.RoundToInt(Random.Range(0,fail.Length - 1))],pos);
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
		Debug.Log ("spawn");

	}

	// Use this for initialization
	void Start () {
		loadPlans ();
		createLevel (3);
	}
	
	// Update is called once per frame
	void Update () {
        if (passWalls.Count != 0)
        {
            timeSinceWall = 0f;
        }
        else
        {
            timeSinceWall += Time.deltaTime;
            if (timeSinceWall > wallSpawnTime)
            {
                SpawnWall();
            }
        }
	}

    void SpawnWall()
    {
        GameObject passWallInstance = (GameObject)Instantiate(passWall, new Vector3(0, 0, 200), transform.rotation);
        passWallInstance.GetComponent<PassWall>().Setup(nextPassWallType, endOnFailTest, velocity.z);
    }
}
