using UnityEngine;
using System.Collections;

public class MirrorHuman : MonoBehaviour {

  public Transform t_Head;
  public Transform t_HandL;
  public Transform t_HandR;

  public GameObject Body;
  public GameObject Head;
  public GameObject HandL;
  public GameObject HandR;

  private Vector3 v1;
  private Vector3 v2;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    

    v1 = t_Head.transform.TransformPoint(new Vector3( 0 , -.1f , 0 ));
    
    Body.transform.localScale = new Vector3( v1.y * .2f ,  v1.y * .1f , v1.y * .2f );
    
    Head.transform.position = transform.TransformPoint( t_Head.position );
    Head.transform.eulerAngles = transform.TransformDirection( t_Head.eulerAngles );
    Head.transform.localScale = new Vector3( v1.y * .52f ,  v1.y * .52f , v1.y * .52f );



    HandL.transform.position = transform.TransformPoint( t_HandL.position );
    HandL.transform.eulerAngles = transform.TransformDirection( t_HandL.eulerAngles );
    HandL.transform.localScale = new Vector3( v1.y * .06f ,  v1.y * .06f , v1.y * .06f );

    HandR.transform.position = transform.TransformPoint( t_HandR.position );
    HandR.transform.eulerAngles = transform.TransformDirection( t_HandR.eulerAngles );
    HandR.transform.localScale = new Vector3( v1.y * .06f ,  v1.y * .06f , v1.y * .06f );


  v2 = new Vector3( 0 , v1.y * .15f , 0 );
    v1 = v1 - v2;
    Body.transform.position = transform.TransformPoint( v1 );
	
	}
}
