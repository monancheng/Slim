﻿using UnityEngine;

//using VoxelBusters.NativePlugins;

public class Share : MonoBehaviour
{
    private void Start()
    {
        Defs.ShareVoxel = this;
    }
    
    private void OnEnable()
    {
        GlobalEvents<OnBtnShareClick>.Happened += OnBtnShareClick;
    }
    
    private void OnBtnShareClick(OnBtnShareClick obj)
    {
        ShareClick();
        GlobalEvents<OnBtnShareClick>.Happened -= OnBtnShareClick;
    }

    public void ShareClick()
    {
        var _shareLink = "http://smarturl.it/YummMonsters";

#if UNITY_IOS

        _shareLink = "http://smarturl.it/YummMonsters";
#endif

        var shareText = "Wow! I Just Scored [" + DefsGame.GameBestScore +
                        "] in #YummMonsters! Can You Beat Me? @AppsoluteGames " + _shareLink;


        var _screenShotPath = Application.persistentDataPath + "/promo1.jpg";

        if (Random.value > 0.5f) _screenShotPath = Application.persistentDataPath + "/promo2.jpg";

        ShareImageAtPathUsingShareSheet(shareText, _screenShotPath);
    }

    private void ShareImageAtPathUsingShareSheet(string _shareText, string _screenShotPath)
    {
//		// Create share sheet
//		ShareSheet shareSheet 	= new ShareSheet();
//
//		shareSheet.Text = _shareText;
//		shareSheet.AttachImageAtPath(_screenShotPath);
//
//		// Show composer
//		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
//		NPBinding.Sharing.ShowView(shareSheet, FinishedSharing);
    }
//		Debug.Log("Finished sharing");
//	{

//	void FinishedSharing (eShareResult _result)
//		Debug.Log("Share Result = " + _result);
//	}
}