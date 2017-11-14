﻿using DarkTonic.MasterAudio;
using DoozyUI;
using UnityEngine;

public class ScreenGame : ScreenItem
{
    [SerializeField] private ScreenColorAnimation _screenAnimation;

    private bool _isScreenRateDone;
    private bool _isScreenReviveDone;
    private bool _isScreenShareDone;

    private enum GameState {Init = 0, WaitToStart = 1, Gameplay = 2, GameOver = 3, RedScreen = 4, RedScreenWait = 5, BackToMenu = 6
    }
    private GameState _gameState;

    private bool _isRewardedVideoReadyToShow;
    private bool _isWaitReward;

    private void Start()
    {
        InitUi();
        _gameState = GameState.Init;
    }

    private void Init()
    { 
//        GlobalEvents<AchievementProgress>.Call(new AchievementProgress
//        {
//            Id = GameServicesManager.ACHIEVEMENT_FIRST_WIN, Progress = Statistics.GameBestScore
//        });

        ++Statistics.QuestGameplayCounter;
        PlayerPrefs.SetInt("QUEST_GAMEPLAY_Counter", Statistics.QuestGameplayCounter);

//        GlobalEvents<AchievementProgress>.Call(new AchievementProgress
//        {
//            Id = GameServicesManager.ACHIEVEMENT_MASTER, Progress = Statistics.QuestGameplayCounter
//        });

        ++Statistics.GameplayCounter;

        PlayerPrefs.SetInt("QUEST_THROW_CounterCounter", Statistics.QuestThrowCounter);

//        GlobalEvents<AchievementProgress>.Call(new AchievementProgress
//        {
//            Id = GameServicesManager.ACHIEVEMENT_FiFIELD_OF_CANDIES, Progress = Statistics.QuestThrowCounter
//        });

        // Сохраняемся тут, чтобы не тормозить игру
        PlayerPrefs.SetInt("QUEST_BOMBS_Counter", Statistics.QuestBombsCounter);

//        GlobalEvents<AchievementProgress>.Call(new AchievementProgress
//        {
//            Id = GameServicesManager.ACHIEVEMENT_EXPLOSIVE, Progress = Statistics.QuestBombsCounter
//        });
        
        GlobalEvents<OnBestScoreUpdate>.Call(new OnBestScoreUpdate());

        _isScreenReviveDone = false;
        _isScreenShareDone = false;
        _isScreenRateDone = false;

        if (Statistics.GameplayCounter > 1)
            GlobalEvents<OnGameOverScreenShow>.Call(new OnGameOverScreenShow());
    }

    private void OnEnable()
    {
        GlobalEvents<OnGameOver>.Happened += OnGameOver;
        GlobalEvents<OnStartGame>.Happened += OnStartGame;
        GlobalEvents<OnGiveReward>.Happened += GetReward;
        GlobalEvents<OnGifSaved>.Happened += OnGifSaved;
    }

    public void StartGameByTouch()
    {
        if (_gameState == GameState.WaitToStart && InputController.GetPosition().y > 220)
        {
            GlobalEvents<OnStartGame>.Call(new OnStartGame());
        }
    }

    private void OnGifSaved(OnGifSaved obj)
    {
    }

    private void OnGameOver(OnGameOver e)
    {
        if (_gameState == GameState.GameOver)
            return;

//        Record.DOSave();
        MasterAudio.PlaySoundAndForget("GameOver");

        
        if (Achievements.IS_ACHIEVEMENT_MISS_CLICK == 0)
        {
            ++Statistics.QuestMissCounter;
            PlayerPrefs.SetInt("QUEST_MISS_Counter", Statistics.QuestMissCounter);
//            GlobalEvents<AchievementProgress>.Call(new AchievementProgress
//            {
//                Id = GameServicesManager.ACHIEVEMENT_MISS_CLICK, Progress = Statistics.QuestMissCounter
//            });
        }

        _gameState = GameState.GameOver;
    }

    public void StartGame()
    {
        GlobalEvents<OnStartGame>.Call(new OnStartGame());
    }

    private void OnStartGame(OnStartGame e)
    {
        if (_gameState == GameState.GameOver)
            return;

        MasterAudio.PlaySoundAndForget("GameStart");
//        Record.DORec();

        ++Statistics.QuestThrowCounter;

        GlobalEvents<OnPointsReset>.Call(new OnPointsReset());
//        if (Statistics.GameplayCounter == 1) GlobalEvents<OnPointsShow>.Call(new OnPointsShow());
        _gameState = GameState.Gameplay;
    }

    public void EndCurrentGame()
    {
        /*if (!isScreenReviveDone) {
            isScreenReviveDone = true;
            if (PublishingService.Instance.IsRewardedVideoReady() && Statistics.currentPointsCount >= 4) {
                UIManager.ShowUiElement ("ScreenRevive");
                UIManager.ShowUiElement ("ScreenReviveBtnRevive");
                UIManager.ShowUiElement ("ScreenReviveBtnBack");
                D.Log ("isScreenReviveDone"); 
                Defs.PlaySound (sndShowScreen);
                return;
            }
        }

        if (!isScreenShareDone) {
            isScreenShareDone = true;
            if ((Statistics.currentPointsCount >= 50) && (Statistics.currentPointsCount == Statistics.gameBestScore)) {
                UIManager.ShowUiElement ("ScreenShare");
                UIManager.ShowUiElement ("ScreenShareBtnShare");
                UIManager.ShowUiElement ("ScreenShareBtnBack");
                Defs.PlaySound (sndShowScreen);
                D.Log ("isScreenShareDone"); 
                return;
            }
        }

        if (!isScreenRateDone) {
            isScreenRateDone = true;
            if ((Statistics.rateCounter < 3) && (Statistics.currentPointsCount >= 100)
                && (Statistics.gameplayCounter % 20 == 0)) {
                ++Statistics.rateCounter;
                PlayerPrefs.SetInt ("rateCounter", Statistics.rateCounter);
                UIManager.ShowUiElement ("ScreenRate");
                UIManager.ShowUiElement ("ScreenRateBtnRate");
                UIManager.ShowUiElement ("ScreenRateBtnBack");
                Defs.PlaySound (sndShowScreen);
                D.Log ("isScreenRateDone"); 
                return;
            }
        }*/

        _gameState = GameState.BackToMenu;
    }

    private void Update()
    {
        BtnEscapeUpdate();

        switch (_gameState)
        {
            case GameState.Init:
                Init();
                _gameState = GameState.WaitToStart;
                return;
            case GameState.WaitToStart:
                
                break;
            case GameState.Gameplay:
                // Gameplay
                break;
            case GameState.GameOver:
                _screenAnimation.SetAlphaMax(0.75f);
                _screenAnimation.SetAnimation(false, 0.1f);
                _screenAnimation.Show();
                _screenAnimation.SetAnimation(true, 0.02f);
                _screenAnimation.SetColor(1.0f, 0.21f, 0.21f);
                _screenAnimation.SetAutoHide(true);

                _gameState = GameState.RedScreen;
                break;
            case GameState.RedScreen:
                if (!_screenAnimation.isActiveAndEnabled)
                {
                    _gameState = GameState.RedScreenWait;
                    EndCurrentGame();
                }
                break;
            case GameState.RedScreenWait:
                break;
            case GameState.BackToMenu:

//            if ((Statistics.gameBestScore == Statistics.currentPointsCount)&&(Statistics.gameBestScore != 0)) {
//                Statistics.gameServices.SubmitScore (Statistics.gameBestScore);
//                PlayerPrefs.SetInt ("BestScore", Statistics.gameBestScore);
//            }
                PlayerPrefs.SetInt("coinsCount", Statistics.CoinsCount);

                GlobalEvents<OnShowMenu>.Call(new OnShowMenu());

                _gameState = GameState.Init;
                break;
        }
    }

    public void Revive()
    {
        GlobalEvents<OnShowRewarded>.Call(new OnShowRewarded());
        _isWaitReward = true;
    }

    private void GetReward(OnGiveReward e)
    {
        if (_isWaitReward)
        {
            _isWaitReward = false;
            if (e.IsAvailable)
            {
                _gameState = GameState.Gameplay;

                ReviveClose();
                MasterAudio.PlaySoundAndForget("GUI_Grab");
            }
            else
            {
                ReviveClose();
                _gameState = GameState.BackToMenu;
            }
        }
    }

    public void Share()
    {
        UIManager.HideUiElement("ScreenShare");
        UIManager.HideUiElement("ScreenShareBtnShare");
        UIManager.HideUiElement("ScreenShareBtnBack");
//        if (SystemInfo.deviceModel.Contains("iPad")) Defs.ShareVoxel.ShareClick();
//        else Defs.Share.ShareClick();
        GlobalEvents<OnBtnShareClick>.Call(new OnBtnShareClick());
        MasterAudio.PlaySoundAndForget("GUI_Grab");
        EndCurrentGame();
    }


    public void Rate()
    {
        UIManager.HideUiElement("ScreenRate");
        UIManager.HideUiElement("ScreenRateBtnRate");
        UIManager.HideUiElement("ScreenRateBtnBack");
        MasterAudio.PlaySoundAndForget("GUI_Grab");
//        GlobalEvents<OnBtnRateClick>.Call(new OnBtnRateClick());
        EndCurrentGame();
    }

    public void ReviveClose()
    {
        UIManager.HideUiElement("ScreenRevive");
        UIManager.HideUiElement("ScreenReviveBtnRevive");
        UIManager.HideUiElement("ScreenReviveBtnBack");
        EndCurrentGame();
    }

    public void ShareClose()
    {
        UIManager.HideUiElement("ScreenShare");
        UIManager.HideUiElement("ScreenShareBtnShare");
        UIManager.HideUiElement("ScreenShareBtnBack");
        EndCurrentGame();
    }

    public void RateClose()
    {
        UIManager.HideUiElement("ScreenRate");
        UIManager.HideUiElement("ScreenRateBtnRate");
        UIManager.HideUiElement("ScreenRateBtnBack");
        EndCurrentGame();
    }

    private void BtnEscapeUpdate()
    {
        //if (Input.GetKeyDown (KeyCode.A))
        if (InputController.IsEscapeClicked())   
        if (_gameState == GameState.Gameplay)
            {
                if (_isScreenReviveDone)
                    ReviveClose();
                else if (_isScreenShareDone)
                    ShareClose();
                else if (_isScreenRateDone)
                    RateClose();
                else
                    GameOver();
            }       
    }


    private void GameOver()
    {
        _gameState = GameState.GameOver;
    }
}