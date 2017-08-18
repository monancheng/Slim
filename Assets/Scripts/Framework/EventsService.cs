//--------------------------------------------------------
// Menu
//--------------------------------------------------------

// Завершение игры
public struct OnGameOver
{
}

public struct OnStartGame
{
}

public struct OnShowMenu
{
}

public struct OnHideMenu
{
}

public struct OnShowMenuButtons
{
}

public struct OnHideMenuButtons
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

//--------------------------------------------------------
// ADS
//--------------------------------------------------------
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

// Добавили монеток
public struct OnCoinsAdded
{
    public int Total;
}

//--------------------------------------------------------
// NOTIFICATIONS
//--------------------------------------------------------

// Показываем нотификейшины на экране
public struct OnShowNotifications
{
}

// Получили нового персонажа
public struct OnGotNewCharacter
{
}

//--------------------------------------------------------
// BUTTONS CLICKS
//--------------------------------------------------------

// Нажали на кнопку "Оценить игру"
public struct OnBtnRateClick
{
}

// Нажали на кнопку "Поделиться игрой"
public struct OnBtnShareClick
{
}

// Нажали на кнопку "Поделиться Gif"
public struct OnBtnShareGifClick
{
    public int CoinsCount;
}

// Нажали на кнопку "Получить подарок"
public struct OnBtnGiftClick
{
    public int CoinsCount;
    public bool IsResetTimer;
}

// Нажали на кнопку "Получить подарок за СЛОВО"
public struct OnBtnWordClick
{
    public int CoinsCount;
    public bool IsResetTimer;
}

// Нажали на кнопку "Купить рандомный скин"
public struct OnBtnGetRandomSkinClick
{
}

// Показать на экран Скин, который игрок получает после нажатия на ленточку "Получить скин за 200 монет"
public struct OnBuySkin
{
    public int Id;
}

// Показать на экран Скин, который игрок получает после нажатия на ленточку "Получить скин"
public struct OnGiftSkin
{
    public int Id;
}

// Изменяем Скин
public struct OnChangeSkin
{
    public int Id;
}

// Все скины открыты
public struct OnSkinAllOpened
{
}

// На финишном эране нет кнопок для показа
public struct OnNoGameOverButtons
{
}

//--------------------------------------------------------
//						    Gift
//--------------------------------------------------------

// Высыпать на экран горсть монет
public struct OnCoinsAddToScreen
{
    public int CoinsCount;
}

// Закончилась анимация Вручения подарка
public struct OnGiftCollected
{
}

// Показать на экран Скин, который игрок получает после нажатия на ленточку "Получить скин за 200 монет"
public struct OnGiftShowRandomSkinAnimation
{
}

// Скрыть экран подарка
public struct OnHideGiftScreen
{
    public int Type;
}

// Закончили проигрывание анимации подарка
public struct OnGiftAnimationDone
{
}

// Высыпать на экран горсть монет
public struct OnGiftResetTimer
{
    public bool IsResetTimer;
}

//--------------------------------------------------------
//						    WORDS
//--------------------------------------------------------

// Еще есть доступные слова
public struct OnWordsAvailable
{
    public bool IsAvailable;
}

// Собрали новое слово
public struct OnWordUpdateProgress
{
    public string Text;
}

// Собрали новое слово
public struct OnWordCollected
{
    public string Text;
}

// Высыпать на экран горсть монет
public struct OnWordStartTimer
{
}

// Нужно ли ждать пока новое Слово будет доступно
public struct OnWordNeedToWait
{
    public bool IsWait;
}

//--------------------------------------------------------
//							GIF
//--------------------------------------------------------

// На финишном эране нет кнопок для показа
public struct OnGifSetName
{
    public string FilePathWithName;
}

// На финишном эране нет кнопок для показа
public struct OnGifSaved
{
}

// Закончилась анимация Вручения подарка
public struct OnGifShared
{
}