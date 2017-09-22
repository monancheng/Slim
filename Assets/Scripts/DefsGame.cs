using PrefsEditor;
using UnityEngine;

public struct DefsGame
{
    public static readonly string AndroidAppID = "com.crazylabs.monsteryumm";
    public static readonly string iOSApp_ID = "id1192223024";
    public static BillingManager IAPs;
    public static GameServicesManager GameServices;
    public static int GameplayCounter = 0; // Считает количество игр сыгранных в этой игровой сессии
    public static int CurrentPointsCount = 0;
    public static int GameBestScore; // Лучший счет
    public static int CoinsCount; // Количество очков игрока
    public static int CurrentFaceId;
    public static int FacesGeneralMin = 0;
    public static int FacesGeneralMax = 14;
    public static int[] FaceAvailable = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    
    public static int SKIN_FACEBOOK = 15;
    public static int SKIN_TWITTER = 16;
    public static int SKIN_INSTAGRAM = 17;
    public static int SKIN_SPONSOR = 18;
      
//    public static readonly int[] FacePrice = {200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200};
    public static int BTN_GIFT_HIDE_DELAY = 0;
    public static int BTN_GIFT_HIDE_DELAY_COUNTER;

//    public static readonly int[] BTN_GIFT_HIDE_DELAY_ARR = {1, 2, 5, 10, 15, 20, 25, 30, 60};
    public static readonly int[] BTN_GIFT_HIDE_DELAY_ARR = {1, 1, 1, 1, 1, 1, 1, 1, 3, 3};

    public static int CurrentScreen = 0;
    public static int SCREEN_MENU = 0;
    public static int SCREEN_GAME = 1;
    public static int SCREEN_SKINS = 2;
    public static int SCREEN_IAPS = 3;
    public static int SCREEN_SETTINGS = 4;
    public static int SCREEN_EXIT = 10;
    public static int SCREEN_NOTIFICATIONS = 11;

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
    public static int QUEST_THROW_Counter;
    public static int QUEST_CHARACTERS_Counter;
    public static int QUEST_BOMBS_Counter;
    public static int QUEST_MISS_Counter;

    public static int RateCounter;

    public static void LoadVariables()
    {
        SecurePlayerPrefs.PassPhrase = "squaredino.com";
        SecurePlayerPrefs.UseSecurePrefs = true;
        SecurePlayerPrefs.AutoConvertUnsecurePrefs = true;
        
        CurrentFaceId = SecurePlayerPrefs.GetInt("currentFaceID");
//      CurrentFaceId = 0;
        GameBestScore = SecurePlayerPrefs.GetInt("BestScore");
//      gameBestScore = 0;
        CoinsCount = SecurePlayerPrefs.GetInt("coinsCount");
        CoinsCount = 2000;
        RateCounter = PlayerPrefs.GetInt("rateCounter", 0);

//        for (var i = 0; i < FaceAvailable.Length; i++)
//            SecurePlayerPrefs.SetInt("faceAvailable_" + i, 0);
        
        for (var i = 0; i < FaceAvailable.Length; i++)
            if (i == 0)
                FaceAvailable[0] = 1;
            else FaceAvailable[i] = SecurePlayerPrefs.GetInt("faceAvailable_" + i);

        for (var i = 0; i < FaceAvailable.Length; i++)
        {
            if (FaceAvailable[i] == 1) ++QUEST_CHARACTERS_Counter;
        }

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
        // TEMP
        QUEST_GAMEPLAY_Counter = 0;
        QUEST_THROW_Counter = PlayerPrefs.GetInt("QUEST_THROW_Counter", 0);
        
        QUEST_BOMBS_Counter = PlayerPrefs.GetInt("QUEST_BOMBS_Counter", 0);
        QUEST_MISS_Counter = PlayerPrefs.GetInt("QUEST_MISS_Counter", 0);
    }
}