using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;

public class GPGSManager : MonoBehaviour

{



	void Awake()
	{
		{
			#if UNITY_ANDROID

			PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
				.EnableSavedGames()
				.Build();

			PlayGamesPlatform.InitializeInstance(config);
			PlayGamesPlatform.DebugLogEnabled = true;
			PlayGamesPlatform.Activate();

			#elif UNITY_IOS

			GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);

			#endif
		}
	}


	public bool SignIn()
	{
		#if UNITY_ANDROID
		bool result = false;
		PlayGamesPlatform.Instance.Authenticate((bool success) =>
			{
				if (success)
				{
					// to do ...
					result = true;
					// 구글 플레이 게임 서비스 로그인 성공 처리
				}
				else
				{
					result = false;
					// to do ...
					// 구글 플레이 게임 서비스 로그인 실패 처리
				}
			});
		
		#elif UNITY_IOS

		Social.localUser.Authenticate((bool success) =>
		{
		if (success)
		{
		// to do ...
		// 애플 게임 센터 로그인 성공 처리
		}
		else
		{
		// to do ...
		// 애플 게임 센터 로그인 실패 처리
		}
		});

		#endif

		return result;
	}
		
	public void ReportScore(int score)
	{
		#if UNITY_ANDROID

		PlayGamesPlatform.Instance.ReportScore(score, GPGSIds.leaderboard_leader_board /*GPGSIds.leaderboard_score*/, (bool success) =>
			{


				if (success)
				{
					Debug.Log("ReportScore success");
					// Report 성공
					// 그에 따른 처리
				}
				else
				{
					Debug.Log("ReportScore fail");
					// Report 실패
					// 그에 따른 처리
				}
			});

		#elif UNITY_IOS

		Social.ReportScore(score, "Leaderboard_ID", (bool success) =>
		{
		if (success)
		{
		// Report 성공
		// 그에 따른 처리
		}
		else
		{
		// Report 실패
		// 그에 따른 처리
		}
		});

		#endif
	}


	public void ShowLeaderboardUI()
	{


		// Sign In 이 되어있지 않은 상태라면
		// Sign In 후 리더보드 UI 표시 요청할 것
		if (Social.localUser.authenticated == false)
		{
			Social.localUser.Authenticate((bool success) =>
				{
					if (success)
					{
						// Sign In 성공
						// 바로 리더보드 UI 표시 요청
						Debug.Log("ShowLeaderboardUI success");
						Social.ShowLeaderboardUI();

						return;
					}
					else
					{
						Debug.Log("ShowLeaderboardUI fail");
						// Sign In 실패 
						// 그에 따른 처리
						return;
					}
				});
		}

		#if UNITY_ANDROID
		PlayGamesPlatform.Instance.ShowLeaderboardUI();
		#elif UNITY_IOS
		GameCenterPlatform.ShowLeaderboardUI("Leaderboard_ID", UnityEngine.SocialPlatforms.TimeScope.AllTime);
		#endif
	}

	public void LoadScoresCallback(IScore[] iscoreVar)
	{
		Debug.Log ("LoadScoresCallback");
		for (int i = 0; i < iscoreVar.Length; i++) {
			Debug.Log (iscoreVar.GetValue (i).ToString ());
		}

	}

	public void GetUsersScore()
	{
		
		PlayGamesPlatform.Instance.LoadScores (
			GPGSIds.leaderboard_leader_board,
			LeaderboardStart.PlayerCentered,
			3,
			LeaderboardCollection.Public,
			LeaderboardTimeSpan.AllTime,
			(LeaderboardScoreData data) => {
				Debug.Log("LeaderboardScoreData information start");


				Debug.Log ("data.Valid: " + data.Valid);  //data.Valid: True
				Debug.Log ("data.Id: " + data.Id); //data.Id: CgkIv4X7_csFEAIQA
				Debug.Log ("data.PlayerScore: " + data.PlayerScore); // data.PlayerScore: GooglePlayGames.PlayGamesScore
				Debug.Log ("data.PlayerScore.userID: " + data.PlayerScore.userID); // data.PlayerScore.userID: g12356694937530055616
				Debug.Log ("data.PlayerScore.formattedValue: " + data.PlayerScore.formattedValue); // data.PlayerScore.formattedValue: 488
				Debug.Log ("data.PlayerScore.leaderboardID: " + data.PlayerScore.leaderboardID); // data.PlayerScore.leaderboardID:
				Debug.Log ("data.PlayerScore.value: " + data.PlayerScore.value); // data.PlayerScore.value: 488


				Debug.Log("LeaderboardScoreData information end");
			});


		//Social.LoadScores(GPGSIds.leaderboard_leader_board, LoadScoresCallback);

	}





	public bool isProcessing
	{
		get;
		private set;
	}
	public string loadedData
	{
		get;
		private set;
	}
	private const string m_saveFileName = "game_save_data";

	public bool isAuthenticated
	{
		get
		{
			return Social.localUser.authenticated;
		}
	}


	private void ProcessCloudData(byte[] cloudData)
	{
		if (cloudData == null)
		{
			Debug.Log("No Data saved to the cloud");
			return;
		}

		string progress = BytesToString(cloudData);
		loadedData = progress;
	}

	public void HEEJUN(string abc){
		Debug.Log ("HEEJUN");
		Debug.Log ("abc: " + abc);
	}


	public void LoadFromCloud(Action<string> afterLoadAction)
	{
		Debug.Log("Load Form Cloud Func Entered");
		if (isAuthenticated && !isProcessing)
		{
			Debug.Log("Load Form Cloud auth success");
			StartCoroutine(LoadFromCloudRoutin(afterLoadAction));
		}
		else
		{
			Debug.Log("auth fail isAuthenticated == " + isAuthenticated + " && isProcessing == " + isProcessing);
			SignIn();
		}
	}

	private IEnumerator LoadFromCloudRoutin(Action<string> loadAction)
	{
		isProcessing = true;
		Debug.Log("Loading game progress from the cloud.");

		((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
			m_saveFileName, //name of file.
			DataSource.ReadCacheOrNetwork,
			ConflictResolutionStrategy.UseLongestPlaytime,
			OnFileOpenToLoad);

		while(isProcessing)
		{
			yield return null;
		}
		Debug.Log ("loadfrom cloud Routine coroutine finished");
		loadAction.Invoke(loadedData);
	}

	public void SaveToCloud(string dataToSave)
	{

		if (isAuthenticated)
		{
			Debug.Log("SaveToCloud Authenticated");
			loadedData = dataToSave;
			isProcessing = true;
			((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(m_saveFileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnFileOpenToSave);
		}
		else
		{
			SignIn();
		}
	}

	private void OnFileOpenToSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
	{
		if (status == SavedGameRequestStatus.Success)
		{

			byte[] data = StringToBytes(loadedData);

			SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();

			SavedGameMetadataUpdate updatedMetadata = builder.Build();

			((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(metaData, updatedMetadata, data, OnGameSave);
			Debug.Log ("OnFileOpenToSave Success");
		}
		else
		{
			Debug.LogWarning("Error opening Saved Game" + status);
		}
	}


	private void OnFileOpenToLoad(SavedGameRequestStatus status, ISavedGameMetadata metaData)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(metaData, OnGameLoad);
			Debug.LogWarning("opening Saved Game Success");
		}
		else
		{
			Debug.LogWarning("Error opening Saved Game" + status);
		}
	}


	private void OnGameLoad(SavedGameRequestStatus status, byte[] bytes)
	{
		if (status != SavedGameRequestStatus.Success)
		{
			Debug.LogWarning("Error Saving" + status);
		}
		else
		{
			ProcessCloudData(bytes);
		}

		isProcessing = false;
	}

	private void OnGameSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
	{
		if (status != SavedGameRequestStatus.Success)
		{
			Debug.LogWarning("Error Saving" + status);
		}

		isProcessing = false;
	}

	private byte[] StringToBytes(string stringToConvert)
	{
		return Encoding.UTF8.GetBytes(stringToConvert);
	}

	private string BytesToString(byte[] bytes)
	{
		return Encoding.UTF8.GetString(bytes);
	}


}