// Menu
// Завершение игры

public struct OnGameOver
{
}

public struct OnStartGame
{
}

// Игрок заработал очки
public struct OnPointsAdd
{
    public int PointsCount;
}

// Показать индикатор очков (+ Анимация)
public struct OnPointsShow
{
}

// Сросить идикатор очков
public struct OnPointsReset
{
}

//----------
// ADS
//----------
// Дать награду игроку
public struct OnGiveReward
{
    public bool IsAvailable;
}

// Rewarded реклама готова к показу
public struct OnRewardedAvailable
{
    public bool IsAvailable;
}

// Запрос на показ рекламы 
public struct OnShowVideoAds
{
    public bool IsAvailable;
}

// Запрос на показ видео рекламы
public struct OnShowRewarded
{
    public bool IsAvailable;
}

// Можно дарить подарок
public struct OnGiftAvailable
{
    public bool IsAvailable;
}

//----------
// NOTIFICATIONS
//----------

// Показываем нотификейшины на экране
public struct OnShowNotifications
{
}

// Получили нового персонажа
public struct OnGotNewCharacter
{
}

//----------
// BUTTONS CLICKS
//----------

// Нажали на кнопку "Оценить игру"
public struct OnBtnRateClick
{
}

// Нажали на кнопку "Поделиться игрой"
public struct OnBtnShareClick
{
}

// Нажали на кнопку "Получить подарок"
public struct OnBtnGiftClick
{
    public int CoinsCount;
}

// Нажали на кнопку "Купить рандомный скин"
public struct OnBtnGetRandomSkinClick
{
}