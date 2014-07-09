using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class Cell {
	public int x, z ;

	public Cell(int x_, int z_){
		x = x_;
		z = z_;
	}

	public Cell(){
		x = -1;
		z = -1;
	}
}

class Pair {
	public float h ;
	public Cell Value ;
	
	public Pair(float h_, Cell c){
		h = h_;
		Value = c;
	}
	
	public Pair(){
		h = Mathf.Infinity;
		Value = new Cell ();
	}
}

public class Astar : MonoBehaviour {
	public bool dilateObstacle = false ;
	public float cellSize = 0.5f;

	public bool testAstar = false;
	public Transform source;
	public Transform destination;
	private Transform oldSource;
	private Transform oldDest;
	private List<Vector3> pathToDraw;
	
	private float minX, maxX, minZ, maxZ;

	private bool[,] walkable ;
	private int height;
	private int width;
	// Use this for initialization
	void Awake () {
		minX = collider.bounds.min.x;
		minZ = collider.bounds.min.z;
		maxX = collider.bounds.max.x;
		maxZ = collider.bounds.max.z;

		height = Mathf.RoundToInt((maxZ - minZ) / cellSize);
		width = Mathf.RoundToInt ((maxX - minX) / cellSize);

		walkable = new bool[width, height];

		for(int x=0 ; x<width ; ++x){
			for(int z=0 ; z<height ; ++z){
				Vector3 position = new Vector3(minX+cellSize*x,0,minZ+cellSize*z);
				NavMeshHit hit ;
				walkable[x,z] = NavMesh.SamplePosition(position, out hit, 0.01f, -1) ;
			}
		}

		if(dilateObstacle){
			for(int x=1 ; x<width-1 ; ++x){
				for(int z=1 ; z<height-1 ; ++z){
					walkable[x,z] = walkable[x,z] && walkable[x+1,z-1] && walkable[x+1,z] && 
									walkable[x+1,z+1] && walkable[x,z+1] ;
				}
			}

			for(int x=width-2 ; x>0 ; --x){
				for(int z=height-2 ; z>0 ; --z){
					walkable[x,z] = walkable[x-1,z+1] && walkable[x-1,z] && 
									walkable[x-1,z-1] && walkable[x,z-1] ;
				}
			}
		}
	}

	public bool getPath(Vector3 source, Vector3 dest, out List<Vector3> path){
		path = new List<Vector3> ();
		path.Add (dest);

		int xSource = Mathf.RoundToInt((source.x - minX) / cellSize) ;
		int zSource = Mathf.RoundToInt((source.z - minZ) / cellSize) ;

		int xDest = Mathf.RoundToInt((dest.x - minX) / cellSize) ;
		int zDest = Mathf.RoundToInt((dest.z - minZ) / cellSize) ;

		//Debug.Log (xSource + ", " + zSource + ", " + xDest + ", " + zDest);

		if (xSource == xDest && zSource == zDest)
			return true;

		Cell[,] parent = new Cell[width, height];
		float[,] g = new float[width, height];
		bool[,] closed = new bool[width, height];

		for(int x=0 ; x<width ; ++x){
			for(int z=0 ; z<height ; ++z){
				parent[x,z] = new Cell() ;
				g[x,z] = Mathf.Infinity ;
				closed[x,z] = false ;
			}
		}
		g[xSource, zSource] = 0;

		List<Pair> openSet = new List<Pair> ();

		openSet.Add (new Pair (0, new Cell (xSource, zSource)));

		bool found = false;
		while (openSet.Count != 0 && !found) {
			Cell c = openSet[0].Value ;
			float score = openSet[0].h;
			openSet.RemoveAt(0) ;
			closed[c.x, c.z] = true ;
			
			//Debug.Log ("visited: " + c.x + ", " + c.z + " : " + score);
			
			for(int i=-1 ; i<=1 && !found ; ++i){
				for(int j=-1 ; j<=1 && !found ; ++j){
					int xn = c.x+i;
					int zn = c.z+j;

					//Debug.Log ("neighboor: " + xn + ", " + zn);

					if(xn<0 || xn>=width || zn<0 || zn >=height)
						continue;

					if(closed[xn, zn])
						continue;

					if(!walkable[xn, zn])
						continue;

					float gFromCur = g[c.x, c.z] + Mathf.Sqrt (Mathf.Abs(i) + Mathf.Abs(j)) ;
					float f = Mathf.Sqrt ((xn - xDest) * (xn - xDest) + (zn - zDest) * (zn - zDest));
					float h = gFromCur+f ;

					//Debug.Log (gFromCur + " + " + f + " = " + h);

					if(parent[xn,zn].x == -1){ // not in open set

						//Debug.Log ("not in openSet");

						parent[xn,zn] = c ;
						g[xn,zn] = gFromCur;

						if(xn == xDest && zn == zDest){
							found = true ;
							break;
						}

						//if(openSet.Count == 0)
						//	openSet.Insert(0, new Pair(h, new Cell(xn,zn)));
						//else {
							int k = 0;
							while(k<openSet.Count && openSet[k].h<h)
								++k;
							openSet.Insert(k, new Pair(h, new Cell(xn, zn))) ;
						//}
					} else {
						
						//Debug.Log ("in openSet");

						int l = 0;
						while(l<openSet.Count && (openSet[l].Value.x != xn || openSet[l].Value.z != zn) )
							++l;

						if(openSet[l].h > h){
							Pair neighboor = openSet[l];
							openSet.RemoveAt(l) ;
							neighboor.h = h;
							parent[neighboor.Value.x,neighboor.Value.z] = c ;
							g[neighboor.Value.x,neighboor.Value.z] = gFromCur ;
							//if(openSet.Count == 0)
							//	openSet.Insert(0, neighboor);
							//else {
								int k = 0;
								while(k<openSet.Count && openSet[k].h<h)
									++k;
								openSet.Insert(k, neighboor) ;
							//}
						}
					}
				}
			}
		}

		if (found) {
			Cell c = parent[xDest, zDest];
			while(c.x != xSource || c.z != zSource){
				path.Insert (0, new Vector3(minX + cellSize * c.x, 0, minZ + cellSize * c.z)) ;
				c = parent[c.x, c.z];
			}
			return true;
 		} else {
			return false;
		}
	}
	
	void OnDrawGizmosSelected() {
		for(int x=0 ; x<width ; ++x){
			for(int z=0 ; z<height ; ++z){
				Vector3 position = new Vector3(minX+cellSize*x,0,minZ+cellSize*z);
				Gizmos.color = walkable[x,z] ? Color.green : Color.red;
				if(!walkable[x,z])
					Gizmos.DrawSphere(position, 0.1f);
			}
		}

		if (testAstar) {
			Gizmos.color = Color.blue ;
			if(oldSource != source || oldDest != destination){
				getPath(source.position, destination.position, out pathToDraw);
				oldSource = source;
				oldDest = destination;
			}
			Vector3 from = source.position;
			foreach (Vector3 position in pathToDraw){
				Gizmos.DrawLine(from, position);
				from = position;
			}
		}
	}
}
