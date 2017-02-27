using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSelectUniforms : MonoBehaviour {

	public GameObject selectorObject;
	public GameObject hand1;
	public GameObject hand2;
	public GameObject tInterface;

	public bool centerObj;

	private MeshRenderer mr;
	// Use this for initialization
	void Start () {
		mr = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

		mr.material.SetVector( "_Selector", transform.InverseTransformPoint( selectorObject.transform.position ));
		//mr.material.SetInt( "_SelectorMoving" , selectorObject.GetComponent<MoveByController>().inside );
		mr.material.SetVector( "_Hand1", transform.InverseTransformPoint( hand1.transform.position ));
		mr.material.SetVector( "_Hand2", transform.InverseTransformPoint( hand2.transform.position ));
		mr.material.SetVector( "_Center", transform.InverseTransformPoint( tInterface.transform.position ));

		if( centerObj ){
			mr.material.SetInt( "_CenterObj", 1);
		}else{
			mr.material.SetInt( "_CenterObj", 0);
		}

		if( selectorObject.GetComponent<MoveByController>().inside ){
			mr.material.SetInt( "_SelectorMoving" , 1 );
		}else{
			mr.material.SetInt( "_SelectorMoving" , 0 );
		}
		
		
	}
}
