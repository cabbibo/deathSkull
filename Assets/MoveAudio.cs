using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAudio : MonoBehaviour {

	public AudioClip TouchClip;
	public AudioClip SelectClip;
	public AudioClip ReleaseClip;
	public AudioClip LoopClip;

	private AudioSource audioPlay;
	private AudioSource audioLoop;

	private MoveByController move;

	private bool oMoving;
	private bool oInside;


	// Use this for initialization
	void Start () {
		
		audioPlay = gameObject.AddComponent<AudioSource>() as AudioSource; //<AudioSource>();
		audioLoop = gameObject.AddComponent<AudioSource>() as AudioSource; //<AudioSource>();

		audioLoop.clip  = LoopClip;
		audioLoop.loop = true;
		audioLoop.volume = 0;

		audioPlay.volume = .4f;

		audioLoop.Play();

		move = GetComponent<MoveByController>();
	}
	
	// Update is called once per frame
	void Update () {


		if( oInside == false && move.inside == true ){
			playAudio( TouchClip );
		}

		if( oMoving == false &&  move.moving == true ){
			playAudio(SelectClip);
		}

		if( oMoving == true &&  move.moving == false ){
			//playAudio(ReleaseClip);
		}

		if( move.moving == true ){
			audioLoop.volume += .01f;
		}else{
			audioLoop.volume -= .01f;
			
		}

		audioLoop.volume = Mathf.Clamp(audioLoop.volume, 0,1);// .01f;

		oInside = move.inside;
		oMoving = move.moving;
		
	}

	void playAudio( AudioClip c ){
		audioPlay.clip = c;
		audioPlay.Play();
	}


}
