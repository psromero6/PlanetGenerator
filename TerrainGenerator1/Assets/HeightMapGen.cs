using UnityEngine;
using System.Collections;

public class HeightMapGen : MonoBehaviour {
	public Terrain terrain;
	private float[,] h=new float[100,100];
	private float[,] heights;
	// Use this for initialization
	void Start () {
		int y, x,xRes,yRes;
		xRes = terrain.terrainData.heightmapWidth;
		yRes = terrain.terrainData.heightmapHeight;
		heights=new float[xRes,yRes];
		for (y = 0; y<yRes; y++) {
			for (x = 0; x <xRes; x++) {
				heights[x,y] = Random.Range(0.0f, 0.02f) * .5f; 
			}
		}

		terrain.terrainData.SetHeights(0, 0, heights);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
