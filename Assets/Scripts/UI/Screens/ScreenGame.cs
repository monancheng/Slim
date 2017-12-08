using System.Collections.Generic;
using DarkTonic.MasterAudio;
using DoozyUI;
using PrefsEditor;
using UnityEngine;
using UnityEngine.Analytics;

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
        
        GlobalEvents<OnGameOver>.Happened += OnGameOver;
        GlobalEvents<OnStartGame>.Happened += OnStartGame;
        GlobalEvents<OnGiveReward>.Happened += GetReward;
        GlobalEvents<OnGifSaved>.Happened += OnGifSaved;
    }

    private void Init()
    { 
//        GlobalEvents<AchievementProgress>.Call(new AchievementProgress
//        {
//            Id = GameServicesManager.ACHIEVEMENT_FIRST_WIN, Progress = PrefsManager.GameBestScore
//        });

        ++PrefsManager.QuestGameplayCounter;
        PlayerPrefs.SetInt("QUEST_GAMEPLAY_Counter", PrefsManager.QuestGameplayCounter);

//        GlobalEvents<AchievementProgress>.Call(new AchievementProgress
//        {
//            Id = GameServicesManager.ACHIEVEMENT_MASTER, Progress = PrefsManager.QuestGameplayCounter
//        });

        ++PrefsManager.GameplayCounter;

        PlayerPrefs.SetInt("QUEST_THROW_CounterCounter", PrefsManager.QuestThrowCounter);

//        GlobalEvents<AchievementProgress>.Call(new AchievementProgress
//        {
//            Id = GameServicesManager.ACHIEVEMENT_FiFIELD_OF_CANDIES, Progress = PrefsManager.QuestThrowCounter
//        });

        // Сохраняемся тут, чтобы не тормозить игру
        PlayerPrefs.SetInt("QUEST_BOMBS_Counter", PrefsManager.QuestBombsCounter);

//        GlobalEvents<AchievementProgress>.Call(new AchievementProgress
//        {
//            Id = GameServicesManager.ACHIEVEMENT_EXPLOSIVE, Progress = PrefsManager.QuestBombsCounter
//        });
        
        GlobalEvents<OnBestScoreUpdate>.Call(new OnBestScoreUpdate());

        _isScreenReviveDone = false;
        _isScreenShareDone = false;
        _isScreenRateDone = false;

        if (PrefsManager.GameplayCounter > 1)
            GlobalEvents<OnGameOverScreenShow>.Call(new OnGameOverScreenShow());
    }

    public void StartGameByTouch()
    {
        if (_gameState == GameState.WaitToStart && InputController.GetPosition().y > 220)
        {
            GlobalEvents<OnStartGame>.Call(new OnStartGame());
            Analytics.CustomEvent("StartGameByTouch",
                new Dictionary<string, object> {{"sessions", PrefsManager.GameplayCounter}});
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
            ++PrefsManager.QuestMissCounter;
            PlayerPrefs.SetInt("QUEST_MISS_Counter", PrefsManager.QuestMissCounter);
//            GlobalEvents<AchievementProgress>.Call(new AchievementProgress
//            {
//                Id = GameServicesManager.ACHIEVEMENT_MISS_CLICK, Progress = PrefsManager.QuestMissCounter
//            });
        }

        _gameState = GameState.GameOver;
    }

    public void StartGame()
    {
        GlobalEvents<OnStartGame>.Call(new OnStartGame());
        Analytics.CustomEvent("MainManuBtnPlayClick",
            new Dictionary<string, object> {{"sessions", PrefsManager.GameplayCounter}});
    }

    private void OnStartGame(OnStartGame e)
    {
        if (_gameState == GameState.GameOver)
            return;

        MasterAudio.PlaySoundAndForget("GameStart");
//        Record.DORec();

        ++PrefsManager.QuestThrowCounter;

        GlobalEvents<OnPointsReset>.Call(new OnPointsReset());
//        if (PrefsManager.GameplayCounter == 1) GlobalEvents<OnPointsShow>.Call(new OnPointsShow());
        _gameState = GameState.Gameplay;
    }

    public void EndCurrentGame()
    {
        /*if (!isScreenReviveDone) {
            isScreenReviveDone = true;
            if (PublishingService.Instance.IsRewardedVideoReady() && PrefsManager.currentPointsCount >= 4) {
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
            if ((PrefsManager.currentPointsCount >= 50) && (PrefsManager.currentPointsCount == PrefsManager.gameBestScore)) {
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
            if ((PrefsManager.rateCounter < 3) && (PrefsManager.currentPointsCount >= 100)
                && (PrefsManager.gameplayCounter % 20 == 0)) {
                ++PrefsManager.rateCounter;
                PlayerPrefs.SetInt ("rateCounter", PrefsManager.rateCounter);
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

//            if ((PrefsManager.gameBestScore == PrefsManager.currentPointsCount)&&(PrefsManager.gameBestScore != 0)) {
//                PrefsManager.gameServices.SubmitScore (PrefsManager.gameBestScore);
//            }
                SecurePlayerPrefs.SetInt("coinsCount", PrefsManager.CoinsCount.GetValue());

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
        UIManager.HideUiElement("ScreenRateMr");
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
        UIManager.HideUiElement("ScreenRateMe");
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