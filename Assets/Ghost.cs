using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Normal.Realtime;

public class Ghost : MonoBehaviour {

	public GameObject player;

	public GameObject handL;
	public GameObject handR;
	public GameObject head;

	// Use this for initialization
	void Awake () {

		transform.parent = GameObject.Find("[CameraRig]").transform;

		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = new Vector3( 1,1,1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.isWriting){

            	//print("hello");
                stream.SendNext( player.GetComponent<Player>().head.transform.localPosition      );
                stream.SendNext( player.GetComponent<Player>().head.transform.localRotation      );
                stream.SendNext( player.GetComponent<Player>().leftHand.transform.localPosition  );
                stream.SendNext( player.GetComponent<Player>().leftHand.transform.localRotation  );
                stream.SendNext( player.GetComponent<Player>().rightHand.transform.localPosition );
                stream.SendNext( player.GetComponent<Player>().rightHand.transform.localRotation );

                //head.transform.localPosition  = player.GetComponent<Player>().head.transform.localPosition      ; 
				//head.transform.localRotation  = player.GetComponent<Player>().head.transform.localRotation      ; 
				//handL.transform.localPosition = player.GetComponent<Player>().leftHand.transform.localPosition  ;
				//handL.transform.localRotation = player.GetComponent<Player>().leftHand.transform.localRotation  ;
				//handR.transform.localPosition = player.GetComponent<Player>().rightHand.transform.localPosition ;
				//handR.transform.localRotation = player.GetComponent<Player>().rightHand.transform.localRotation ;

            }else{
            	//print("gettin recaeved");
                head.transform.localPosition =    (Vector3)stream.ReceiveNext();
                head.transform.localRotation =    (Quaternion)stream.ReceiveNext();   
                handL.transform.localPosition =    (Vector3)stream.ReceiveNext();
                handL.transform.localRotation =    (Quaternion)stream.ReceiveNext();   
                handR.transform.localPosition =    (Vector3)stream.ReceiveNext();
                handR.transform.localRotation =    (Quaternion)stream.ReceiveNext();   
            }        
        }
}
