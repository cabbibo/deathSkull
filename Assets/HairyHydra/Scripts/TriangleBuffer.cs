using UnityEngine;
using System.Collections;

public class TriangleBuffer : MonoBehaviour {

	public GameObject meshObject;

	public int triCount;
	public ComputeBuffer _triBuffer;	
	public int[] triangles;

	// Use this for initialization
	void Awake () {

		Mesh m = meshObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
	
		triangles =  m.triangles;
		triCount = m.triangles.Length;

		_triBuffer = new ComputeBuffer( triCount , sizeof(int) ); 
		_triBuffer.SetData(triangles);
	
	}

	void OnDisable(){
		ReleaseBuffer();
	}

	public void ReleaseBuffer(){
    _triBuffer.Release(); 
  }

	
	// Update is called once per frame
	void Update () {
	
	}
}
