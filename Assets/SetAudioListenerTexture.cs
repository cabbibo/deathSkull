using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAudioListenerTexture : MonoBehaviour {

	public AudioListenerTexture al;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Renderer>().material.SetTexture("_AudioMap", al.AudioTexture);
	}
}
