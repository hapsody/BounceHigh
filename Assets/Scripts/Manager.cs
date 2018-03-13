using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Manager : MonoBehaviour, IGameObject {

	static private Manager _instance;

	static public Manager Instance { get { return _instance; } set { _instance = value;} }
	[SerializeField]
	private GameObject[] _heightNumbers = new GameObject[3];
	private int _height = 0;
	private int _bestHeight =0;

	private bool _bplay = true;
	public bool BPLAY { get { return _bplay; } set { _bplay = value; } }
	[SerializeField]
	private Sphere _sphere = null;

	[SerializeField]
	private GameObject _bgResourceCircle;
	[SerializeField]
	private GameObject[] _cubes = new GameObject[2];
	private int turn = 0;

	[SerializeField]
	private GameObject _sampleCube;


	private Vector3 _mouseInPos;
	private Vector3 _mouseOutPos;

	[SerializeField]
	private GameObject _camera = null;

	private bool _mouseClicked = false;
	private float _distanceX;
	private float _lastCameraPositionY;


	void Awake() {
		_instance = this;
	}

	// Use this for initialization
	void Start () {
		GameInit ();
	}
	void GameInit()
	{
		
		_camera.transform.localPosition = Vector3.Slerp (_camera.transform.localPosition, new Vector3 (0, -5, -10), Time.deltaTime * 3);
		_lastCameraPositionY = 0f;

		_sphere.transform.position = new Vector3 (0, 0, 0);
		_sphere.SphereResume ();
		_mouseClicked = false;
		_mouseInPos = _mouseOutPos = new Vector3 (0, 0, 0);
		_sampleCube.transform.position = _cubes [0].transform.position = new Vector3 (0, 0, -20);
		_bplay = true;

	}

	void GameStop(){
		_bplay = false;
		_sphere.SphereFreeze ();
	}

	public void GameUpdate()
	{

		if (_sphere.transform.position.x < -10 || _sphere.transform.position.x > 10 || _sphere.transform.position.y < _lastCameraPositionY - 20) 
			GameStop ();

		_height = (int) (_sphere.transform.position.y + 8.2f);
		/*
		_heightNumbers[0].GetComponent<TextMesh>().text =  (_height).ToString() + "m";
		var spherePosition =  _sphere.transform.position;

		_heightNumbers [0].transform.position = new Vector3(spherePosition.x-1.5f, spherePosition.y+3, -1);
*/
		var spherePosition =  _sphere.transform.position;
		_heightNumbers [1].transform.position = new Vector3(spherePosition.x, spherePosition.y+2, -1);
		//_heightNumbers [1].transform.position = new Vector3 (_camera.transform.position.x, _camera.transform.position.y  - 8, -1);
		if (_height > _bestHeight) {
			_heightNumbers [1].GetComponent<TextMesh> ().text = (_height).ToString () + "m";
			_bestHeight = _height;
		}

		if (_bplay) {
			
			if (Input.GetMouseButtonDown (0)) {
				_mouseInPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				_mouseClicked = true;
			}

			if (Input.GetMouseButtonUp (0)) {
				
				_mouseOutPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				_distanceX = Vector3.Distance (_mouseOutPos, _mouseInPos);

				_cubes [turn].transform.position = _sampleCube.transform.position;
				_cubes [turn].transform.localScale = _sampleCube.transform.localScale;
				_cubes [turn].transform.rotation = _sampleCube.transform.rotation;

				_sampleCube.transform.position = new Vector3 (0, 0, -20);
				_mouseClicked = false;

			}

			if (_mouseClicked) {
				_mouseOutPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				_distanceX = Vector3.Distance (_mouseOutPos, _mouseInPos) * 2;
				float cubeLengthLimit = 7;
				if (_distanceX < cubeLengthLimit) {
					_sampleCube.transform.position = new Vector3 (_mouseInPos.x, _mouseInPos.y, 0);
					_sampleCube.transform.localScale = new Vector3 (_distanceX, 0.1f, 1.0f);
				} else {
					_sampleCube.transform.localScale = new Vector3 (cubeLengthLimit, 0.1f, 1.0f);
				}

				var relativePos = _mouseOutPos - _mouseInPos;

				Vector3 vec3AxisX = new Vector3 (1.0f, 0.0f, 0.0f);
				Quaternion quaternion = Quaternion.FromToRotation (vec3AxisX, relativePos);
				_sampleCube.transform.rotation = quaternion;

			} else {
				_mouseOutPos.z = -10;
				_lastCameraPositionY = _mouseOutPos.y + 1;
				_camera.transform.localPosition = Vector3.Slerp (_camera.transform.localPosition, new Vector3 (0, _lastCameraPositionY, -10), Time.deltaTime * 3);
			}
		} else {
			if (Input.GetMouseButtonUp (0)) {
				GameInit ();
			}
		}
	}

	// Update is called once per frame
	void Update () {
		GameUpdate ();
	}



		
}
