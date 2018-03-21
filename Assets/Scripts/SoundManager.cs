﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public AudioClip _BGM; //재생할 소리를 변수로 담습니다.
	AudioSource myAudio; //AudioSorce 컴포넌트를 변수로 담습니다.
	public static SoundManager instance;  //자기자신을 변수로 담습니다.
	void Awake() //Start보다도 먼저, 객체가 생성될때 호출됩니다
	{
		if (SoundManager.instance == null) //incetance가 비어있는지 검사합니다.
		{
			SoundManager.instance = this; //자기자신을 담습니다.
		}
	}
	void Start () {
		myAudio = this.gameObject.GetComponent<AudioSource>(); //AudioSource 오브젝트를 변수로 담습니다.
		PlaySound();

	}
	public void PlaySound()
	{
		myAudio.loop = true;
		myAudio.PlayOneShot(_BGM); //soundExplosion을 재생합니다.
	}
	void Update () {

	}
}
