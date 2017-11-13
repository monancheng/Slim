using PrefsEditor;
using UnityEngine;

public class Statistics : MonoBehaviour {
	public const int GameVersion = 0;

	public static int GameplayCounter = 0; // Считает количество игр сыгранных в этой игровой сессии
	public static int CurrentPointsCount = 0;
	public static int GameBestScore; // Лучший счет
	public static int CoinsCount; // Количество очков игрока  
    
	public static bool IsFirstBuy; // Первая покупка была произведена 

	public static int QuestGameplayCounter;
	public static int QuestThrowCounter;
	public static int QuestCharactersCounter;
	public static int QuestBombsCounter;
	public static int QuestMissCounter;

	public static int RateCounter;
	
	// Use this for initialization
	void Awake () {
		SecurePlayerPrefs.PassPhrase = "squaredino.com";
		SecurePlayerPrefs.UseSecurePrefs = true;
		SecurePlayerPrefs.AutoConvertUnsecurePrefs = true;
        
		IsFirstBuy = SecurePlayerPrefs.GetBool("IsFirstBuy");
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

		QuestGameplayCounter = PlayerPrefs.GetInt("QUEST_GAMEPLAY_Counter", 0);
		// TEMP
		QuestGameplayCounter = 0;
        
		QuestThrowCounter = PlayerPrefs.GetInt("QUEST_THROW_Counter", 0);
		QuestBombsCounter = PlayerPrefs.GetInt("QUEST_BOMBS_Counter", 0);
		QuestMissCounter = PlayerPrefs.GetInt("QUEST_MISS_Counter", 0);
	}
}
