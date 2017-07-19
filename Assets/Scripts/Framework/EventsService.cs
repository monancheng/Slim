// Menu
// Завершение игры
public struct OnGameOver {}
public struct OnStartGame {}
// Игрок заработал очки
public struct OnPointsAdd { public int PointsCount; }
// Показать индикатор очков (+ Анимация)
public struct OnPointsShow {}
// Сросить идикатор очков
public struct OnPointsReset {}

// ADS
// Дать награду игроку
public struct OnGiveReward { public bool IsAvailable; }
// Rewarded реклама готова к показу
public struct OnRewardedAvailable { public bool IsAvailable; }

// Запрос на показ рекламы 
public struct OnShowVideoAds { public bool IsAvailable; }
// Запрос на показ видео рекламы
public struct OnShowRewarded { public bool IsAvailable; }

// Можно дарить подарок
public struct OnGiftAvailable { public bool IsAvailable; }
// Показываем нотификейшины на экране
public struct OnShowNotifications {}

//example
//событие с несколькими параметрами
/*public struct GameSettingEvent
{
    public bool useAds;
    public bool useAnalytics;
    public float coinsFactor;
    public int startingCoins;
}*/