using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class SphereGen : MonoBehaviour {
	public Mesh meshin;
	// Use this for initialization

	public Vector3[] newVertices;
	public Vector2[] newUV;
	public int[] newTriangles;






	void Start () {


		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = meshin.vertices;
		mesh.uv = newUV;
		mesh.triangles = meshin.triangles;



		Vector3[] verticies = mesh.vertices;
		int[] triangles = mesh.triangles;



		Debug.Log (mesh.vertexCount+";"+verticies[120].x);
		divideAll (verticies, triangles,mesh);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void normalize(Vector3[] arr){
		float ls= 0;
		foreach (Vector3 vec in arr) {
		ls+=vec.magnitude;	
		}

		for(int i=0;i<arr.Length;i++) {
			Vector3 vec=arr[i];
			vec.x/=ls;
			vec.y/=ls;
			vec.z/=ls;
		}

		return;
	}

	void divideAll(Vector3[] verticies, int[] triangles,Mesh mesh){
		//int newTriangleCount = triangles.Length;
		List <Vector3> vertArray= new List<Vector3>();
		List<int> newTriangles= new List<int>();
		for(int i=0;i<triangles.Length;i+=3){
			Vector3 tvert1;
			Vector3 tvert2;
			Vector3 tvert3;
//			tvert.x=verticies[triangles[i]];
//			tvert.y=verticies[triangles[i+1]];
//			tvert.z=verticies[triangles[i+2]];

			vertArray.AddRange(verticies);
			newTriangles.AddRange(PaigeUtils.SubArray<int>(triangles,0,i));

			tvert1=verticies[triangles[i]];
			tvert2=verticies[triangles[i+1]];
			tvert3=verticies[triangles[i+2]];

			//Debug.Log("tvert"+tvert1+";"+tvert2+";"+tvert3+";"+i);
			Vector3[] abc=new Vector3[3];

			abc[0]=((tvert1+tvert3)*0.5f);
			abc[1]=((tvert1+tvert2)*0.5f);
			abc[2]=((tvert2+tvert3)*0.5f);
			//Debug.Log(abc[0]+";"+abc[1]+";"+abc[2].z);
			
			foreach(Vector3 vec in abc){
				vec.Normalize();
			}
			vertArray.AddRange(abc);
			int ai=vertArray.IndexOf(abc[0]);
			int bi=vertArray.IndexOf(abc[1]);
			int ci=vertArray.IndexOf(abc[2]);



			newTriangles.AddRange(new int[9]{triangles[i],ai,bi,bi,triangles[1],ci,ai,bi,ci,ai,ci,triangles[2]});
			newTriangles.AddRange(PaigeUtils.SubArray<int>(triangles,i+3,triangles.Length-(i+3)));
			//Debug.Log("normalized"+abc[0]+";"+abc[1]+";"+abc[2].x);
			triangles=newTriangles.ToArray;
			verticies=vertArray.ToArray;


		}

		mesh.triangles = triangles;
		mesh.vertices = verticies;

	
	
	}

}
