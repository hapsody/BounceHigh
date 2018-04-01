using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Manager : MonoBehaviour, IGameObject {

	[SerializeField]
	private Transform _musicCC = null;
	[SerializeField]
	private Text _composeTitle = null;
	[SerializeField]
	private Text _musicTitle = null;
	static private Manager _instance;

	static public Manager Instance { get { return _instance; } set { _instance = value;} }

	private int _height = 0;
	private int _bestHeight =0;

	[SerializeField]
	private GameObject _background = null;

	private bool _replay = false;
	private bool _bplay = false;
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
	private Text _bestScoreTitle;
	[SerializeField]
	private Text _levelTitle;
	[SerializeField]
	private Text _scoreTitle;

	private int _level;
	public int Level { get { return _level; } set { _level = value; } }


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


	private int _playCount = 0;

	[SerializeField]
	private GameObject _block = null;
	private List<GameObject> _blockList = new List<GameObject>();
	private float _minHeight;

	[SerializeField]
	private UnityAdsHelper _unityAdsHelper = null;

	[SerializeField]
	private GPGSManager _gpgsManager = null;

	bool _increaseR = true;
	bool _increaseG = true;
	bool _increaseB = true;




	void Awake() {
		
		Screen.SetResolution( 1440, 2560, true );

	

		_instance = this;
	}

	// Use this for initialization
	void Start () {
		InitMeshFilter ();
		Invoke ("OpeningStart", 3);
	}
		
	public void OpeningStart (){
		StartCoroutine ("PanelFadeIn", true);
		GameStart ();
	}

	IEnumerator ClimbUpCAM()
	{
		while (_bplay) {
			_minHeight += Time.deltaTime * 0.5f *_level;

			if (_minHeight > _lastCameraPositionY + 0) {
				
				float cameraX;
				if (_sphere.transform.position.x > 13.5f)
					cameraX = 13.5f;
				else if ( _sphere.transform.position.x < -13.5f) 
					cameraX = -13.5f;
				else//( _sphere.transform.position.x <= 13.5 && _sphere.transform.position.x >= -13.5)
					cameraX = _sphere.transform.position.x;

				_camera.transform.localPosition = Vector3.Slerp (_camera.transform.localPosition, new Vector3 (cameraX, _minHeight, -10), Time.deltaTime * 3);
			}
			
			yield return new WaitForSeconds (0.01f);
		}
	}

	IEnumerator PanelFadeIn(bool isOpening)
	{
		_panelAlpha = 1f;


		while (true) {

			// just fade out
			if (_panelAlpha > 0)
				_panelAlpha -= Time.deltaTime; 
			else {
				_panelAlpha = 0f;
				if (isOpening) {
					_musicCC.gameObject.SetActive (false);
					// if it's opening, sphere move and setting replay to true after fade in & out done.
					_sphere.SphereResume ();
					_replay = true;
				}
				break;
			}

			Color color = new Color (1, 1, 1, _panelAlpha);
			if (isOpening) {
				_composeTitle.gameObject.GetComponent<Text> ().color = color;
				_musicTitle.gameObject.GetComponent<Text> ().color = color;
			}
			else
				_panel.gameObject.GetComponent<Image> ().color = color;
			yield return new WaitForSeconds (0.01f);
		}
	}

	IEnumerator LevelUp()
	{
		_level = 0;
		while (_bplay) {
			_level++;
			_levelTitle.text = "LEVEL : "+_level;
			var gravity = Physics.gravity;
			Physics.gravity = new Vector3 (0, gravity.y - 2, 0);
			if (_level >= 20)
				break;
			yield return new WaitForSeconds (10);
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
		_scoreTitle.text = "0m";
		_sphereDestroyed = false;
		_mouseOutPos = new Vector3 (0, 0, -10);
		_minHeight = 4.5f;
		BlockRemover ();
		_playCount++;
		Physics.gravity = new Vector3 (0, -60, 0);

	}

	public void GameStart() {
		GameInit ();

		Color color = new Color (1, 1, 1, 0);
		_panel.gameObject.GetComponent<Image> ().color = color;
		_title.gameObject.SetActive (true);
		_panel.gameObject.SetActive (true);
		_playCount = 0;
		_bplay = false;
		_background.GetComponent<SpriteRenderer> ().material.color = new Vector4 (Random.Range (0.2f, 0.8f), Random.Range (0.2f, 0.8f), Random.Range (0.2f, 0.8f), 1f);
		_gpgsManager.SignIn ();

	}

	public void GameStop(){
		_bplay = false;
		_bestScoreTitle.gameObject.SetActive (false);
		_levelTitle.gameObject.SetActive (false);

		_sphere.SphereFreeze ();
		int temp = PlayerPrefs.GetInt ("_bestScore");
	
		if (temp < _bestHeight) {
			PlayerPrefs.SetInt ("_bestScore", _bestHeight);
			PlayerPrefs.Save ();
			_gpgsManager.ReportScore (_bestHeight);
		}



		if (_pausePanel.gameObject.activeSelf == false)			
			_pausePanel.gameObject.SetActive (true);

		if (!_sphereDestroyed && _sphere.transform.position.x < -26) {
			_particleSystem.transform.position = new Vector3 (-27f, _sphere.transform.position.y, -1);
			_particleSystem.Play ();
			_sphereDestroyed = true;
		} else if (!_sphereDestroyed &&_sphere.transform.position.x > 26) {
			_particleSystem2.transform.position = new Vector3 (27f, _sphere.transform.position.y, -1);
			_particleSystem2.Play ();
			_sphereDestroyed = true;

		} 
	}

	IEnumerator BlockGenerator()
	{
		while (_bplay) {
			_block.transform.position = new Vector3 (Random.Range (_camera.transform.position.x - 14f, _camera.transform.position.y + 14f), _camera.transform.position.y + 30, 0);
			_block.transform.localScale = new Vector3 (0.01f, 1, Random.Range (2f, 3f));
			_blockList.Add (GameObject.Instantiate (_block));
			if(_level < 20)
				yield return new WaitForSeconds (Random.Range (4 - _level * 0.2f, 5f - _level * 0.2f));
			else
				yield return new WaitForSeconds (Random.Range (0.1f,0.5f));
		}
	}

	public void BlockRemover()
	{
		for (int i = _blockList.Count-1; i >= 0; i--) {
			Destroy (_blockList [i]);
			_blockList.RemoveAt (i);
		}
	}

	public void AmbientBackgroundEffect(){
		Color color = _background.GetComponent<SpriteRenderer> ().material.color;

		if (color.r < 0.1f) {
			color.r = 0.1f;
			_increaseR = true;
		} else if (color.r > 0.9f) {
			color.r = 0.9f;
			_increaseR = false;
		}

		if( _increaseR )
			color.r += Random.Range (0.001f, 0.01f);
		else
			color.r -= Random.Range (0.001f, 0.01f);


		if (color.b < 0.1f) {
			color.b = 0.1f;
			_increaseB = true;
		} else if (color.r > 0.9f) {
			color.b = 0.9f;
			_increaseB = false;
		}

		if( _increaseB )
			color.b += Random.Range (0.001f, 0.01f);
		else
			color.b -= Random.Range (0.001f, 0.01f);


		if (color.g < 0.1f) {
			color.g = 0.1f;
			_increaseG = true;
		} else if (color.g > 0.9f) {
			color.g = 0.9f;
			_increaseG = false;
		}

		if( _increaseG)
			color.g += Random.Range (0.001f, 0.01f);
		else
			color.g -= Random.Range (0.001f, 0.01f);


		_background.GetComponent<SpriteRenderer> ().material.color = color;

	}

	public void InitMeshFilter()
	{
		MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = new Mesh ();
	}

	public void DrawTriangle(Vector3 pos)
	{
		MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
		MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
		Mesh mesh = new Mesh();

		//var currentPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		pos.z = 1;
		mesh.vertices = new Vector3[]
		{
			pos, new Vector3(pos.x + 2, pos.y-0.5f, 1), new Vector3(pos.x + 2, pos.y+ 0.5f, 1)
		};

		//삼각형 그리는 순서 설정
		mesh.triangles = new int[]{2,1,0};
		mesh.RecalculateNormals();
		meshFilter.mesh = mesh;
	}

	public void GameUpdate()
	{
		if (_bplay) {
			AmbientBackgroundEffect ();

			if (_sphere.transform.position.x < -28.2 || _sphere.transform.position.x > 28.2 || _sphere.transform.position.y < _minHeight - 27) {//_lastCameraPositionY - 20)
				GameStop ();

			}

			_height = (int)(_sphere.transform.position.y + 8.2f);
			if (_height > _bestHeight) {
				_bestHeight = _height;
				_scoreTitle.text = _bestHeight.ToString () + "m";
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
				_lastCameraPositionY = _mouseOutPos.y - 5;
				if (_minHeight < _lastCameraPositionY) {
					_minHeight = _lastCameraPositionY;

					float cameraX;
					if (_sphere.transform.position.x > 14f)
						cameraX = 14f;
					else if (_sphere.transform.position.x < -14f)
						cameraX = -14f;
					else//( _sphere.transform.position.x <= 13.5 && _sphere.transform.position.x >= -13.5)
					cameraX = _sphere.transform.position.x;
				
					_camera.transform.localPosition = Vector3.Slerp (_camera.transform.localPosition, new Vector3 (cameraX, _lastCameraPositionY + 0, -10), Time.deltaTime * 3);
					
						
				}
			}
		} else { // when game is stopped
			//_mouseOutPos.z = -10;
			_lastCameraPositionY = _mouseOutPos.y -5;
			//_camera.transform.localPosition = Vector3.Slerp (_camera.transform.localPosition, new Vector3 (0, _lastCameraPositionY+5, -10), Time.deltaTime * 3);

			if (Input.GetMouseButtonUp (0) && _replay) {
				_scoreTitle.gameObject.SetActive (true);
				_bestScoreTitle.gameObject.SetActive (true);
				_levelTitle.gameObject.SetActive (true);
				int temp = PlayerPrefs.GetInt ("_bestScore");
				_bestScoreTitle.text = "Best : "+ temp.ToString () + "m";

				if (_playCount >= 12) {
					_unityAdsHelper.ShowAds ();
					_playCount = 0;
				}
				_panel.gameObject.SetActive (true);
				_title.gameObject.SetActive (false);
				StartCoroutine ("PanelFadeIn", false);

				GameInit ();
				_bplay = true;
				StartCoroutine ("ClimbUpCAM");
				StartCoroutine ("BlockGenerator");
				StartCoroutine ("LevelUp");
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
		//_gpgsManager.SignIn ();
		//_gpgsManager.ReportScore (_bestHeight);
		_gpgsManager.ShowLeaderboardUI ();


		//_gpgsManager.SaveToCloud("Score:100, x:10, y:10, z:20");

	}

	public void GetScoreButton(){
		_gpgsManager.GetUsersScore ();


		//_gpgsManager.LoadFromCloud (_gpgsManager.HEEJUN);
	}


}
