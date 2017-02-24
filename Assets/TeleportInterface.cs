using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportInterface : MonoBehaviour {

	public GameObject[] teleportButtons;
	private bool buttonsShown = true;
	private bool canTouch = true;
	// Use this for initialization
	void Start () {
		for( int i = 0; i < teleportButtons.Length; i++ ){
			teleportButtons[i].GetComponent<TeleporterScale>().tInterface = this;//transform.gameObject.GetComponent<Tele;
		}

		
	}

	public void Teleport(){
		HideButtons();
		//t.Teleport();
		canTouch = false;
		Invoke("Cooldown", 1 );
	}

	void HideButtons(){
		
		for( int i = 0; i < teleportButtons.Length; i++ ){
			teleportButtons[i].GetComponent<MeshRenderer>().enabled = false;
			teleportButtons[i].GetComponent<Collider>().enabled = false;
		}

		buttonsShown = false;

	}

	void ShowButtons(){
		
		for( int i = 0; i < teleportButtons.Length; i++ ){
			teleportButtons[i].GetComponent<MeshRenderer>().enabled = true;
			teleportButtons[i].GetComponent<Collider>().enabled = true;
		}

		buttonsShown = true;

	}

	void Cooldown(){
		canTouch = true;
	}

	void ToggleButtons(){
		if( buttonsShown == false ){
			ShowButtons();
		}else{
			HideButtons();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider c){
		if( c.tag == "internal"){
	   		ToggleButtons();
	   	}

	}
}
