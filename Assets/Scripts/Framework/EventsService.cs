﻿//--------------------------------------------------------
// Menu
//--------------------------------------------------------

// Завершение игры
public struct OnGameOver{}

public struct OnStartGame{}

public struct OnShowMenu{}

public struct OnShowScreenCoins{}

public struct OnScreenCoinsHide{}

public struct OnHideMenu{}

public struct OnShowMenuButtons{}

public struct OnHideMenuButtons{}

public struct OnRateScreenShow{}

// Игрок заработал очки
public struct OnPointsAdd
{
    public int PointsCount;
}

// Сросить идикатор очков
public struct OnPointsReset{}

// Проиграть анимацию BestScore
public struct OnBestScoreUpdate{}

//--------------------------------------------------------
// Game Input - объект, который принимает клики по игровому
// полю и посылает их в игру
//--------------------------------------------------------

public struct OnGameInputEnable
{
    public bool Flag;
}

//--------------------------------------------------------
// ADS
//--------------------------------------------------------

// Показываем Video рекламу, если доступна
public struct OnAdsVideoTryShow {}
// Запрос на показ рекламы 
public struct OnShowVideoAds {}
// Начался показ Video рекламы
public struct OnAdsVideoShowing {}
// Отключить рекламу
public struct OnAdsDisable {}

// Rewarded реклама зарузилась
public struct OnRewardedLoaded { public bool IsAvailable; }
// Rewarded реклама готова к показу (Время ожидания завершилось)
public struct OnRewardedWaitTimer { public bool IsWait; }
// Запрос на показ видео рекламы
public struct OnShowRewarded {}
// Начался показ Rewarded рекламы
public struct OnAdsRewardedShowing {}
// Дать награду игроку
public struct OnGiveReward
{
    public bool IsAvailable;
}

// Можно дарить подарок
public struct OnGiftAvailable
{
    public bool IsAvailable;
}

// Добавляем монетки
public struct OnCoinsAdd
{
    public int Count;
}

// Добавили монеток
public struct OnCoinsAdded
{
    public int Total;
}

// Смотрим видео и получаем скин
public struct OnAdsRewardedBuySkin
{
    public int Id;
}


// Купили скин за просмотр видео
public struct OnBuySkinByRewarded
{
    public int Id;
}

//--------------------------------------------------------
// NOTIFICATIONS
//--------------------------------------------------------

// Показываем нотификейшины на экране
public struct OnGameOverScreenShow{}
// Показываем активные нотификейшины на экране
public struct OnGameOverScreenShowActiveItems{}
// Скрываем активные нотификейшины на экране
public struct OnGameOverScreenHideActiveItems{}

// Получили нового персонажа
public struct OnGotNewCharacter{}

//--------------------------------------------------------
// BUTTONS CLICKS
//--------------------------------------------------------

// Нажали на кнопку "Поделиться игрой"
public struct OnBtnShareClick{}

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
public struct OnBtnGetRandomSkinClick{}

// Показать на экран Скин, который игрок получает после нажатия на ленточку "Получить скин за 200 монет"
public struct OnBuySkin
{
    public int Id;
}

// Купили скин за реальные деньги
public struct OnBuySkinByIAP
{
    public int Id;
}

// Разблокировать все скины и отключить рекламу
public struct OnSkinsUnlockAll{}

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
public struct OnSkinAllOpened{}

// Все GENERAL скины открыты
public struct OnSkinAllGeneralOpened{}

// На финишном эране нет кнопок для показа
public struct OnNoGameOverButtons{}

//--------------------------------------------------------
//						    Gift
//--------------------------------------------------------

// Высыпать на экран горсть монет
public struct OnCoinsAddToScreen
{
    public int CoinsCount;
}

// Закончилась анимация Вручения подарка
public struct OnGiftCollected{}

// Показать на экран Скин, который игрок получает после нажатия на ленточку "Получить скин за 200 монет"
public struct OnGiftShowRandomSkinAnimation{}

// Скрыть экран подарка
public struct OnHideGiftScreen{}

// Закончили проигрывание анимации подарка
public struct OnGiftAnimationDone{}

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

// Стартуем новый таймер (ждем слово)
public struct OnWordStartTimer{}

// Обнулить таймер слов (нужно, чтобы начать собирать новое слово немедленно)
public struct OnWordResetTimer{}

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
public struct OnGifSaved{}

// Закончилась анимация Вручения подарка
public struct OnGifShared{}

// Спрятать все Tube
public struct OnHideTubes{}

//--------------------------------------------------------
//							IAPs
//--------------------------------------------------------
public struct OnIAPsBuySkin
{
    public int Id;
}

//--------------------------------------------------------
//							Tube Manager
//--------------------------------------------------------

public struct OnTubeCreateExample{}


