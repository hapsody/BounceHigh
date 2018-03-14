using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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

	[SerializeField]
	private Text _bestScore;
	[SerializeField]
	private Transform _panel;
	private float _panelAlpha = 0f;
	private bool _alphaIncrease = true;
	[SerializeField]
	private ParticleSystem _particleSystem = null;
	[SerializeField]
	private ParticleSystem _particleSystem2 = null;
	private bool _sphereDestroyed = false;

	void Awake() {
		_instance = this;
	}

	// Use this for initialization
	void Start () {
		GameInit ();
	}

	IEnumerator PanelFadeIn()
	{
		_panelAlpha = 1f;
		while (true) {
			/* // fade in and out
		if (_alphaIncrease){
			if (_panelAlpha < 1)
				_panelAlpha += Time.deltaTime * 2;
			else {
				_panelAlpha = 1f;
				_alphaIncrease = false;
			}
		} 
		else {
			if (_panelAlpha > 0)
				_panelAlpha -= Time.deltaTime * 2; 
			else {
				_panelAlpha = 0f;
				_alphaIncrease = true;
				break;
			}
		}
		*/
			// just fade out
			if (_panelAlpha > 0)
				_panelAlpha -= Time.deltaTime; 
			else {
				_panelAlpha = 0f;
				_alphaIncrease = true;
				break;
			}

			Color color = new Color (1, 1, 1, _panelAlpha);
			_panel.gameObject.GetComponent<Image> ().color = color;
			yield return new WaitForSeconds (0.01f);
		}
	}

	void GameInit()
	{
		_lastCameraPositionY = 0f;
		_sphere.transform.position = new Vector3 (0, 0, 0);
		_mouseClicked = false;
		_mouseInPos = _mouseOutPos = new Vector3 (0, 0, 0);
		_sampleCube.transform.position = _cubes [0].transform.position = new Vector3 (0, 0, -20);
		_bestHeight = 0;
		_bestScore.text = "0m";
		_sphereDestroyed = false;

	}

	void GameStop(){
		_bplay = false;
		_sphere.SphereFreeze ();
	}


	public void GameUpdate()
	{

		if (_sphere.transform.position.x < -13 || _sphere.transform.position.x > 13 || _sphere.transform.position.y < _lastCameraPositionY - 25) {
			GameStop ();

			if (!_sphereDestroyed && _sphere.transform.position.x < -13) {
				_particleSystem.transform.position = new Vector3 (-12.5f, _sphere.transform.position.y, -1);
				_particleSystem.Play ();
				_sphereDestroyed = true;
			} else if (!_sphereDestroyed &&_sphere.transform.position.x > 13) {
				_particleSystem2.transform.position = new Vector3 (12.5f, _sphere.transform.position.y, -1);
				_particleSystem2.Play ();
				_sphereDestroyed = true;

			} 

		}

		if (_bplay) {

			_height = (int) (_sphere.transform.position.y + 8.2f);
			if (_height > _bestHeight ) {
				_bestHeight = _height;
				_bestScore.text = _bestHeight.ToString() + "m";
			}

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
				_camera.transform.localPosition = Vector3.Slerp (_camera.transform.localPosition, new Vector3 (0, _lastCameraPositionY+5, -10), Time.deltaTime * 3);
			}
		} else {
			_mouseOutPos.z = -10;
			_lastCameraPositionY = _mouseOutPos.y + 1;
			_camera.transform.localPosition = Vector3.Slerp (_camera.transform.localPosition, new Vector3 (0, _lastCameraPositionY+5, -10), Time.deltaTime * 3);

			if (Input.GetMouseButtonUp (0)) {
				_panel.gameObject.SetActive (true);
				StartCoroutine ("PanelFadeIn");

				GameInit ();
				_bplay = true;
				_sphere.SphereResume ();
			}
		}
	}

	// Update is called once per frame
	void Update () {
		GameUpdate ();
	}



		
}
