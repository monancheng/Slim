using PrefsEditor;
using UnityEngine;

public struct DefsGame
{
    public static readonly int GameVersion = 0;
    public static GameServicesManager GameServices;
    public static int GameplayCounter = 0; // Считает количество игр сыгранных в этой игровой сессии
    public static int CurrentPointsCount = 0;
    public static int GameBestScore; // Лучший счет
    public static int CoinsCount; // Количество очков игрока
    public static int CurrentFaceId;
    public static int FacesGeneralMin = 0;
    public static int FacesGeneralMax = 18;
    public static readonly int FacesSocialStartID = FacesGeneralMax+1;
    public static readonly int FacesPaybleStartID = FacesSocialStartID + 4;
    public static int[] FaceAvailable = {
        // General
        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        // Social
        0, 0, 0, 0,
        // Money
        0, 0, 0, 0};

    public static readonly int SKIN_FACEBOOK = FacesSocialStartID;
    public static readonly int SKIN_TWITTER = FacesSocialStartID + 1;
    public static readonly int SKIN_INSTAGRAM = FacesSocialStartID + 2;
    public static readonly int SKIN_SPONSOR = FacesSocialStartID + 3;
    
    public static readonly int IAP_SKIN_1 = FacesPaybleStartID;
    public static readonly int IAP_SKIN_2 = FacesPaybleStartID + 1;
    public static readonly int IAP_SKIN_3 = FacesPaybleStartID + 2;
    public static readonly int IAP_SKIN_4 = FacesPaybleStartID + 3;  

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
//        CoinsCount = 2000;
        RateCounter = PlayerPrefs.GetInt("rateCounter", 0);
        if (RateCounter != 0 && PlayerPrefs.GetInt("RateForVersion", -1) != GameVersion)
        {
            RateCounter = 0;
            PlayerPrefs.SetInt("rateCounter", 0);
        }

//        for (var i = 0; i < FaceAvailable.Length; i++)
//            SecurePlayerPrefs.SetInt("faceAvailable_" + i, 1);
        
        for (var i = 1; i < FaceAvailable.Length; i++)
            FaceAvailable[i] = SecurePlayerPrefs.GetInt("faceAvailable_" + i);

        for (var i = 0; i < FaceAvailable.Length; i++)
        {
            if (FaceAvailable[i] == 1) ++QUEST_CHARACTERS_Counter;
        }

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