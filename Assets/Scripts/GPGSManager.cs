using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
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
					Debug.Log("hapsody success");
					result = true;
					// 구글 플레이 게임 서비스 로그인 성공 처리
				}
				else
				{
					Debug.Log("hapsody failed");
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
					Debug.Log("hapsody report success");
					// Report 성공
					// 그에 따른 처리
				}
				else
				{
					Debug.Log("hapsody report failed");
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
						Social.ShowLeaderboardUI();
						return;
					}
					else
					{
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






}