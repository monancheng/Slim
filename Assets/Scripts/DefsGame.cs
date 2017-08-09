using UnityEngine;

public struct DefsGame
{
    public static int NoAds;
    public static BillingManager IAPs;
    public static GameServicesManager GameServices;
    public static CameraMovement CameraMovement;
    public static ScreenGame ScreenGame;
    public static ScreenCoins ScreenCoins;
    public static ScreenSkins ScreenSkins;
    public static int GameplayCounter = 0; // Считает количество игр сыгранных в этой игровой сессии
    public static int CurrentPointsCount = 0;
    public static int GameBestScore; // Лучший счет
    public static int CoinsCount; // Количество очков игрока
    public static int CurrentFaceId;
    public static int ThrowsCounter = 0;
    public static bool IsCanPlay = true;
    public static int[] FaceAvailable = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    public static readonly int[] FacePrice = {200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200};
    public static int BTN_GIFT_HIDE_DELAY = 0;
    public static int BTN_GIFT_HIDE_DELAY_COUNTER;

//    public static readonly int[] BTN_GIFT_HIDE_DELAY_ARR = {1, 2, 5, 10, 15, 20, 25, 30, 60};
    public static readonly int[] BTN_GIFT_HIDE_DELAY_ARR = {1, 1};

    public static int CurrentScreen = 0;
    public static int SCREEN_MENU = 0;
    public static int SCREEN_GAME = 1;
    public static int SCREEN_SKINS = 2;
    public static int SCREEN_IAPS = 3;
    public static int SCREEN_EXIT = 10;

    public static int IS_ACHIEVEMENT_FIRST_WIN;
    public static int IS_ACHIEVEMENT_NEW_SKIN;
    public static int IS_ACHIEVEMENT_MULTI_PULTI;
    public static int IS_ACHIEVEMENT_MISS_CLICK;
    public static int IS_ACHIEVEMENT_GET_MAX;
    public static int IS_ACHIEVEMENT_THREE_JUMPS;

    public static int IS_ACHIEVEMENT_MASTER;
    public static int IS_ACHIEVEMENT_FiFIELD_OF_CANDIES;
    public static int IS_ACHIEVEMENT_EXPLOSIVE;
    public static int IS_ACHIEVEMENT_COLLECTION;

    public static int QUEST_GAMEPLAY_Counter;
    public static int QUEST_GOALS_Counter;
    public static int QUEST_THROW_Counter;
    public static int QUEST_CHARACTERS_Counter;
    public static int QUEST_BOMBS_Counter;
    public static int QUEST_MISS_Counter;

    public static int RateCounter;
    public static Coins Coins { get; set; }
    public static bool IsNeedToShowCoin = false;


    public static void LoadVariables()
    {
        NoAds = PlayerPrefs.GetInt("noAds", 0);
        CurrentFaceId = PlayerPrefs.GetInt("currentFaceID", 0);
        //currentFaceID = 0;
        GameBestScore = PlayerPrefs.GetInt("BestScore", 0);
        //gameBestScore = 0;
        CoinsCount = PlayerPrefs.GetInt("coinsCount", 0);
        CoinsCount = 201;
        RateCounter = PlayerPrefs.GetInt("rateCounter", 0);

        //loadRewardedClock();
        //loadGiftClock();

        for (var i = 0; i < FaceAvailable.Length; i++)
            PlayerPrefs.SetInt("faceAvailable_" + i, 0);
        
        for (var i = 0; i < FaceAvailable.Length; i++)
            if (i == 0)
                FaceAvailable[0] = 1;
            else FaceAvailable[i] = PlayerPrefs.GetInt("faceAvailable_" + i, 0);

        BTN_GIFT_HIDE_DELAY_COUNTER = PlayerPrefs.GetInt("BTN_GIFT_HIDE_DELAY_COUNTER", 0);
        //BTN_GIFT_HIDE_DELAY_COUNTER = 0;

        IS_ACHIEVEMENT_FIRST_WIN = PlayerPrefs.GetInt("IS_ACHIEVEMENT_FIRST_WIN", 0);
        IS_ACHIEVEMENT_NEW_SKIN = PlayerPrefs.GetInt("IS_ACHIEVEMENT_NEW_SKIN", 0);
        IS_ACHIEVEMENT_MULTI_PULTI = PlayerPrefs.GetInt("IS_ACHIEVEMENT_MULTI_PULTI", 0);
        IS_ACHIEVEMENT_MISS_CLICK = PlayerPrefs.GetInt("IS_ACHIEVEMENT_MISS_CLICK", 0);
        IS_ACHIEVEMENT_GET_MAX = PlayerPrefs.GetInt("IS_ACHIEVEMENT_GET_MAX", 0);
        IS_ACHIEVEMENT_THREE_JUMPS = PlayerPrefs.GetInt("IS_ACHIEVEMENT_THREE_JUMPS", 0);
        IS_ACHIEVEMENT_MASTER = PlayerPrefs.GetInt("IS_ACHIEVEMENT_MASTER", 0);
        IS_ACHIEVEMENT_FiFIELD_OF_CANDIES = PlayerPrefs.GetInt("IS_ACHIEVEMENT_FiFIELD_OF_CANDIES", 0);
        IS_ACHIEVEMENT_EXPLOSIVE = PlayerPrefs.GetInt("IS_ACHIEVEMENT_EXPLOSIVE", 0);
        IS_ACHIEVEMENT_COLLECTION = PlayerPrefs.GetInt("IS_ACHIEVEMENT_COLLECTION", 0);

        QUEST_GAMEPLAY_Counter = PlayerPrefs.GetInt("QUEST_GAMEPLAY_Counter", 0);
        QUEST_GOALS_Counter = PlayerPrefs.GetInt("QUEST_GOALS_Counter", 0);
        QUEST_THROW_Counter = PlayerPrefs.GetInt("QUEST_THROW_Counter", 0);
        QUEST_CHARACTERS_Counter = PlayerPrefs.GetInt("QUEST_CHARACTERS_Counter", 0);
        QUEST_BOMBS_Counter = PlayerPrefs.GetInt("QUEST_BOMBS_Counter", 0);
        QUEST_MISS_Counter = PlayerPrefs.GetInt("QUEST_MISS_Counter", 0);
    }
}