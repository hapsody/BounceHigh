using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Manager : MonoBehaviour, IGameObject {

	static private Manager _instance;

	static public Manager Instance { get { return _instance; } set { _instance = value;} }

	private bool _bplay = true;
	public bool BPLAY { get { return _bplay; } set { _bplay = value; } }
	[SerializeField]
	private Sphere _sphere = null;

	[SerializeField]
	private GameObject _bgResourceCircle;
	[SerializeField]
	private GameObject _cube;
	[SerializeField]
	private GameObject _cube2;
	[SerializeField]
	private GameObject _sampleCube;


	private Vector3 _mouseInPos;
	private Vector3 _mouseOutPos;

	private Vector3 _remainScale;

	[SerializeField]
	private GameObject _camera = null;
	[SerializeField]
	private GameObject _resourceCircle = null;

	private bool _mouseClicked = false;
	private float _distanceX;

	void Awake() {
		_instance = this;
	}

	// Use this for initialization
	void Start () {
		GameInit ();
	}
	void GameInit()
	{
		_camera.transform.localPosition = Vector3.Slerp (_camera.transform.localPosition, new Vector3 (0, 1, -10), Time.deltaTime * 3);
		_sphere.transform.position = new Vector3 (0, 0, 0);
		_sphere.SphereResume ();
		_bplay = true;

	}

	void GameStop(){
		_bplay = false;
		_sphere.SphereFreeze ();
	}

	public void GameUpdate()
	{
		if (_sphere.transform.position.x < -10 || _sphere.transform.position.x > 10)
			GameStop ();


		
		if (_bplay) {
			
			if (Input.GetMouseButtonDown (0)) {
				_mouseInPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				_mouseClicked = true;
				_remainScale = _resourceCircle.transform.localScale;
			}

			if (_mouseClicked) {
				_mouseOutPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

				_distanceX = Vector3.Distance (_mouseOutPos, _mouseInPos);

				_sampleCube.transform.position = new Vector3 ((_mouseInPos.x + _mouseOutPos.x) / 2, (_mouseInPos.y + _mouseOutPos.y) / 2, 0);
				_sampleCube.transform.localScale = new Vector3 (_distanceX, 0.1f, 1.0f);

				//End Pos - Start Pos = Vec1
				var relativePos = _mouseOutPos - _mouseInPos;

				Vector3 vec3AxisX = new Vector3 (1.0f, 0.0f, 0.0f);
				Quaternion quaternion = Quaternion.FromToRotation (vec3AxisX, relativePos);
				_sampleCube.transform.rotation = quaternion;


				// when player is dragging, the part of making resourceCircle
				Vector3 usedEnergyScale = new Vector3 (_distanceX, _distanceX, 0);

				if (_resourceCircle.transform.localScale.x > usedEnergyScale.x * 0.01f) {
					Vector3 currentScale = _resourceCircle.transform.localScale;
					currentScale = _remainScale - usedEnergyScale * 0.1f;
					_resourceCircle.transform.localScale = currentScale;
				} else
					_resourceCircle.transform.localScale = new Vector3 (0f, 0f, 0.01f);



			}

			if (Input.GetMouseButtonUp (0)) {
				_mouseOutPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);


				//_cubes[0] = GameObject.Instantiate (_cube);

				_cube.transform.position = _sampleCube.transform.position;
				_cube.transform.localScale = _sampleCube.transform.localScale;
				_cube.transform.rotation = _sampleCube.transform.rotation;

				_sampleCube.transform.position = new Vector3 (0, 0, -20);
				_distanceX = Vector3.Distance (_mouseOutPos, _mouseInPos);


				_mouseClicked = false;
			}

			if (!_mouseClicked) {
				_mouseOutPos.z = -10;
				_camera.transform.localPosition = Vector3.Slerp (_camera.transform.localPosition, new Vector3 (0, _mouseOutPos.y + 1, -10), Time.deltaTime * 3);
				var cameraPosition = _camera.transform.position;
				_bgResourceCircle.transform.position = _resourceCircle.transform.position = new Vector3 (cameraPosition.x + 6, cameraPosition.y - 12, -3);


				Vector3 prevScale = _resourceCircle.transform.localScale;


				if (prevScale.x < 3) {
					prevScale = prevScale + new Vector3 (0.1f, 0.1f, 0f) * Time.deltaTime; 
					_resourceCircle.transform.localScale = prevScale;
				}
			}
		} else {
			if (Input.GetMouseButtonDown (0)) {
				GameInit ();
			}
		}
	}

	// Update is called once per frame
	void Update () {
		GameUpdate ();
	}



		
}
