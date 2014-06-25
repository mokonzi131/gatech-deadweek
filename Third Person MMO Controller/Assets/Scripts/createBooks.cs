using UnityEngine;
using System.Collections;

public class createBooks : MonoBehaviour {

	public GameObject bookPrefab;

	public int nbBooks = 15;
	public int minX;
	public int maxX;
	public int minZ;
	public int maxZ;
	public int highY;

	// Use this for initialization
	void Awake () {
		for (int i=0; i<nbBooks; ++i) {
			int x = Random.Range(minX, maxX);
			int z = Random.Range(minZ, maxZ);
			GameObject book = Instantiate (bookPrefab, new Vector3(x, highY, z), Quaternion.identity) as GameObject;
		}
	}
}
