using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class HeightMapGen : MonoBehaviour {
	private bool debug=false;
	public Terrain terrain;
	public string infile;
	public GameObject ridgeIndicator;
	//private float[,] h=new float[100,100];
	private float[,] heights;
	private byte[] a16 = new byte[2];
	private List<GameObject> objList;
	private int stepSize=5;
	private float resConversion;
	private List<Node> nodeList=new List<Node>();
	void Start () {

		//byte[] fileData = File.ReadAllBytes(infile);
		FileStream stream = File.Open(infile, FileMode.Open);
		//stream.BeginRead (infile);
		int y, x,xRes,yRes,hRes,tHeight,xt,yt;
		xRes = terrain.terrainData.heightmapWidth;
		yRes = terrain.terrainData.heightmapHeight;
		hRes = terrain.terrainData.heightmapResolution;
		tHeight = terrain.terrainData.heightmapHeight;
		heights=new float[xRes,yRes];
		float i = 0;

		resConversion = (float)hRes/1201;
		Debug.Log (resConversion);
		for (x = 0; x<1201; x++) {
			for (y = 0; y<1201; y++) {
				xt=(int)(x*resConversion);
				yt=(int)(y*resConversion);
				stream.Read(a16,0,2);

				//small endian
			Array.Reverse(a16);
				i=BitConverter.ToInt16(a16,0);
				heights[xt,yt] =(i/(10*tHeight));
				//if(x<10&&y>1100) Debug.Log(heights[xt,yt]+";"+i+";"+x+"->"+xt+";"+y+"->"+yt+";");

			}
			//if(x<10) Debug.Log(i+";"+BitConverter.ToInt16(a16,0)+";"+(char)a16[0]+"."+a16[1]);

		}
		if (debug) {
			Debug.Log("start profileRecognision");		
		}
		profileRecognision (terrain);
		if (debug) {
			Debug.Log("end profileRecognision");		
		}
		Debug.Log(terrain.terrainData.GetHeight (100, 0)+";"+heights[100,0]);
	}
	void profileRecognision(Terrain terrain){
		//N-S recursion
		if (debug) {
			Debug.Log("N-S recursion");		
		}
		for (int i=0; i<terrain.terrainData.heightmapWidth; i+=stepSize) {
						Node startNode = new Node ();
						startNode.x = i;
						startNode.y = 0;
						int direction = 1;
						startNode.height = terrain.terrainData.GetHeight (i, 0);
			nodeList.Add(profileRecognisionRec (terrain, startNode, direction, new List<Node> ()));
				}
		//W-E
		if (debug) {
			Debug.Log("W-E recursion");		
		}
		for (int i=0; i<terrain.terrainData.heightmapWidth; i+=stepSize) {
			Node startNode = new Node ();
			startNode.x = 0;
			startNode.y = i;
			int direction = 2;
			startNode.height = terrain.terrainData.GetHeight (0, i);
			nodeList.Add(profileRecognisionRec (terrain, startNode, direction, new List<Node> ()));
		}
		//NE-SW
		if (debug) {
			Debug.Log("NE-SW recursion");		
		}
		for (int i=0; i<terrain.terrainData.heightmapWidth; i+=stepSize) {
			Node startNode = new Node ();
			startNode.x = i;
			startNode.y = 0;
			int direction = 3;
			startNode.height = terrain.terrainData.GetHeight (i, 0);
			nodeList.Add(profileRecognisionRec (terrain, startNode, direction, new List<Node> ()));
		}
		for (int i=0; i<terrain.terrainData.heightmapWidth; i+=stepSize) {
			Node startNode = new Node ();
			startNode.x = terrain.terrainData.heightmapWidth;
			startNode.y = i;
			int direction = 3;
			startNode.height = terrain.terrainData.GetHeight (i, 0);
			nodeList.Add(profileRecognisionRec (terrain, startNode, direction, new List<Node> ()));
		}

		//NW-SE
		if (debug) {
			Debug.Log("NW-SE recursion");		
		}
		for (int i=0; i<terrain.terrainData.heightmapWidth; i+=stepSize) {
			Node startNode = new Node ();
			startNode.x = 0;
			startNode.y = i;
			int direction = 4;
			startNode.height = terrain.terrainData.GetHeight (i, 0);
			nodeList.Add(profileRecognisionRec (terrain, startNode, direction, new List<Node> ()));
		}
		for (int i=0; i<terrain.terrainData.heightmapWidth; i+=stepSize) {
			Node startNode = new Node ();
			startNode.x = i;
			startNode.y = 0;
			int direction = 4;
			startNode.height = terrain.terrainData.GetHeight (i, 0);
			nodeList.Add(profileRecognisionRec (terrain, startNode, direction, new List<Node> ()));
		}


		polygonElimination (nodeList);

		foreach (Node currNode in nodeList) {
						if (currNode.isRidge) {
								GameObject probe = (GameObject)GameObject.Instantiate (ridgeIndicator);
								probe.transform.Translate ((new Vector3 (currNode.x, (int)currNode.height, currNode.y)));
						}
				}

	}
	
	
	//direction can be 1,2,3,4, corresponding to N-S, W-E, NE-SW, NW-SE
	//scope is an arraylists of a maximum size of 5 that is used to check for ridge potential
	Node profileRecognisionRec(Terrain terrain,Node nodeIn,int direction,List<Node> scope){
		if (debug) {
			Debug.Log("profileRecognisionRec");		
		}
		Node node = new Node ();
		int dx, dy;
		dx=0;
		dy=0;
		switch (direction)
		{
			//not using 0 because we want to make sure that we initalized direction and set it appropriately
		case 1:
			dx=0;
			dy=stepSize;
			node.neibors[0]=nodeIn;
			nodeIn.neibors[4]=node;
			break;
		case 2:
			dx=stepSize;
			dy=0;
			node.neibors[6]=nodeIn;
			nodeIn.neibors[2]=node;
			break;
		case 3:
			dx=-stepSize;
			dy=stepSize;
			node.neibors[1]=nodeIn;
			nodeIn.neibors[5]=node;
			break;
		case 4:
			dx=stepSize;
			dy=stepSize;
			node.neibors[7]=nodeIn;
			nodeIn.neibors[3]=node;
			break;
		default:
			Console.WriteLine("invalid direction input in profileRecognisionRec");
			break;
		}



		node.x =dx+nodeIn.x;
		node.y =dy+nodeIn.y;
		//recursion break
		if (node.x > terrain.terrainData.heightmapWidth ||
		    node.x < 0||
		    node.y > terrain.terrainData.heightmapWidth||
		    node.y < 0 ) {

				return node;
		}

		node.height=terrain.terrainData.GetHeight(node.x,node.y);

		while (scope.Count >= 5) {
			scope.RemoveAt(0);
		}
		scope.Add (node);

		ridgeCheck (scope);

		return profileRecognisionRec(terrain,node,direction,scope);
	
	}


	void ridgeCheck(List<Node> scope){
		if (debug) {
			Debug.Log("ridgeCheck");		
		}
		for (int i=1; i<scope.Count; i++) {
				
			ridgeAheadCheck(scope,i);
		}
	
	}

	void ridgeAheadCheck(List<Node> scope,int ind){
		if (debug) {
			Debug.Log("ridgeAheadCheck");		
		}
		int post=ind+1;
		Node currNode = scope[ind];

		while (post<scope.Count) {
			Node postNode = scope[post];
			if(currNode.height>postNode.height){
				ridgeBehindCheck(scope,ind);
				return;
			}	
			post++;
		}
	
	}

	void ridgeBehindCheck(List<Node> scope,int ind){
		if (debug) {
			Debug.Log("ridgeBehindCheck");		
		}
		int pre=ind-1;
		Node currNode = scope[ind];
		Node preNode = scope[pre];
		while (pre>0) {
			
			if(currNode.height>preNode.height){
				currNode.isRidge=true;
			
				//objList.Add(probe);
				Console.WriteLine(scope);
				return;
			}	
			pre--;
		}
	
	}

	void polygonElimination(List<Node> nodeList){
		nodeList.Sort ();
		foreach(Node node in nodeList){
			if(inClosedPoly(node)) 
				node.isRidge=false;

		}

	}
	bool inClosedPoly(Node node){
		for (int i=0; i<8; i++) {
			if(inClosedPolyHelper(node,node,true)==node){

				return true;
			}

		}
		return false;
	}

	Node inClosedPolyHelper(Node head,Node node,bool start){
		if (node==head&&!start){
			return head;
		}

		for (int i=0; i<8; i++) {
			if (node!=null && node.neibors[i]!=null) {
				if(node.neibors[i].isRidge){
					if(inClosedPolyHelper (head, node.neibors[i],false)==head){
						return head;
					}
				}
		
			}
			//Debug.Log("ClosedPolyHelper running");
		}
		return null;
	
	}


	// Update is called once per frame
	void Update () {
	
	}
	
}
