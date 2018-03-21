
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
	private string _leaderBoardID = "CgkIv4X7_csFEAIQAQ";

	private bool _bplay = true;
	public bool BPLAY { get { return _bplay; } set { _bplay = value; } }
	[SerializeField]
	private Sphere _sphere = null;

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
	private Text _title = null;
	[SerializeField]
	private Text _bestScore;
	[SerializeField]
	private Transform _panel;
	private float _panelAlpha = 0f;
	[SerializeField]
	private ParticleSystem _particleSystem = null;
	[SerializeField]
	private ParticleSystem _particleSystem2 = null;
	private bool _sphereDestroyed = false;
	[SerializeField]
	private Transform _pausePanel;
	private bool _replay = true;

	private int _playCount = 0;

	[SerializeField]
	private GameObject _block = null;
	private List<GameObject> _blockList = new List<GameObject>();
	private float _minHeight;

	[SerializeField]
	private UnityAdsHelper _unityAdsHelper = null;

	[SerializeField]
	private GPGSManager _gpgsManager = null;

	void Awake() {
		_instance = this;
	}

	// Use this for initialization
	void Start () {
		GameStart ();




	}

	/*
	IEnumerator GrowthTree(){
		while (true) {
			//var scale = _tree.transform.localScale;
			//scale = new Vector3 (scale.x + Time.deltaTime * 0.001f, scale.y + Time.deltaTime * 0.001f, 0.1f);
			//_tree.transform.localScale = scale;
			var treeData = _tree.data as TreeEditor.TreeData;
			if (treeData != null) {
				var branchGroups = treeData.branchGroups;
				Debug.Log ("branchlength"+branchGroups.Length);
			}
			yield return new WaitForSeconds (0.02f);
		}
	}
*/


	IEnumerator ClimbUpCAM()
	{
		while (_bplay) {
			_minHeight += Time.deltaTime * 2;

			if (_minHeight > _lastCameraPositionY+5) 
				_camera.transform.localPosition = Vector3.Slerp (_camera.transform.localPosition, new Vector3 (0, _minHeight, -10), Time.deltaTime * 3);
			
			yield return new WaitForSeconds (0.01f);
		}
	}

	IEnumerator PanelFadeIn()
	{
		_panelAlpha = 1f;
		while (true) {

			// just fade out
			if (_panelAlpha > 0)
				_panelAlpha -= Time.deltaTime; 
			else {
				_panelAlpha = 0f;
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
		_mouseOutPos = new Vector3 (0, 0, -10);
		_minHeight = 4.5f;
		BlockRemover ();
		_playCount++;

	}

	public void GameStart() {
		GameInit ();
		_sphere.SphereResume ();
		Color color = new Color (1, 1, 1, 0);
		_panel.gameObject.GetComponent<Image> ().color = color;
		_title.gameObject.SetActive (true);
		_panel.gameObject.SetActive (true);
		_playCount = 0;
		_bplay = false;

	}

	public void GameStop(){
		_bplay = false;
		_sphere.SphereFreeze ();

		if (_pausePanel.gameObject.activeSelf == false)			
			_pausePanel.gameObject.SetActive (true);

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

	IEnumerator BlockGenerator()
	{
		while (_bplay) {
			_block.transform.position = new Vector3 (Random.Range (-11, 11), _camera.transform.position.y + 30, 0);
			_block.transform.localScale = new Vector3 (0.01f, 1, Random.Range (1f, 3f));
			//_block.transform.Rotate (new Vector3 (Random.Range (0, 180), 0, 0));
			_blockList.Add (GameObject.Instantiate (_block));
			yield return new WaitForSeconds (Random.Range (10, 15));
		}
	}

	public void BlockRemover()
	{
		for (int i = _blockList.Count-1; i >= 0; i--) {
			Destroy (_blockList [i]);
			_blockList.RemoveAt (i);
		}
	}

	public void GameUpdate()
	{
		if (_bplay) {

			if (_sphere.transform.position.x < -13 || _sphere.transform.position.x > 13 || _sphere.transform.position.y < _minHeight - 25 )//_lastCameraPositionY - 20)
				GameStop ();

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
				float cubeLengthLimit = 10;
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
				//_mouseOutPos.z = -10;
				_lastCameraPositionY = _mouseOutPos.y + 1;
				if (_minHeight < _lastCameraPositionY + 5) {
			
					_camera.transform.localPosition = Vector3.Slerp (_camera.transform.localPosition, new Vector3 (0, _lastCameraPositionY + 5, -10), Time.deltaTime * 3);
				}
			}
		} else { // when game is stopped
			//_mouseOutPos.z = -10;
			_lastCameraPositionY = _mouseOutPos.y + 1;
			//_camera.transform.localPosition = Vector3.Slerp (_camera.transform.localPosition, new Vector3 (0, _lastCameraPositionY+5, -10), Time.deltaTime * 3);

			if (Input.GetMouseButtonUp (0) && _replay) {
			//if (_replay) {
				if (_playCount >= 12) {
					_unityAdsHelper.ShowAds ();
					_playCount = 0;
				}
				_panel.gameObject.SetActive (true);
				_title.gameObject.SetActive (false);
				StartCoroutine ("PanelFadeIn");

				GameInit ();
				_bplay = true;
				StartCoroutine ("ClimbUpCAM");
				StartCoroutine ("BlockGenerator");
				_sphere.SphereResume ();
				_replay = false;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		GameUpdate ();
	}

	public void ReplayButton(){
		_replay = true;
		_pausePanel.gameObject.SetActive (false);
	}

	public void RankButton(){
		Debug.Log ("rank");
		_gpgsManager.SignIn ();
		_gpgsManager.ReportScore (_bestHeight);
		_gpgsManager.ShowLeaderboardUI ();
		
//		SignIn ();
	//	ShowLeaderboardUI ();
	}

}
