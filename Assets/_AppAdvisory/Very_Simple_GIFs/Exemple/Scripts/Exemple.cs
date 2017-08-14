#pragma warning disable 0162 // code unreached.

#if FACEBOOK_SDK_INSTALLED
using Facebook.Unity;
#endif
#if TWITTER_SDK_INSTALLED
using Fabric.Twitter;
#endif


#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0618 // obslolete
#pragma warning disable 0108 
#pragma warning disable 0649 //never used
#pragma warning disable 0429 //never used

/***********************************************************************************************************
 * Produced by App Advisory - http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/




using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using AppAdvisory.VSGIF;

namespace AppAdvisory.VSGIF.Exemple
{
	public class Exemple : MonoBehaviour 
	{
		public GameObject BtnRec;
		public Text BtnRecText;
		public GameObject BtnReset;
		public GameObject BtnSave;

		bool firstStart = true;

		void Awake()
		{
			BtnRec.SetActive(true);
			BtnReset.SetActive(false);
			BtnSave.SetActive(false);


			#if FACEBOOK_SDK_INSTALLED
			FB.Init ();
			#endif

		}

		void Start()
		{
			Record.OnStartRecord += OnStartRecord;
			Record.OnPauseRecord += OnPauseRecord;
			Record.OnSavedGIFEvent += OnSavedGIFEvent;
			Record.OnResetCurrentRecord += OnResetCurrentRecord;
			Record.OnShareGIFEvent += OnShareGIFEvent;
		}

		void OnStartRecord ()
		{
			print("OnStartRecord");

			BtnRec.SetActive(true);
			BtnReset.SetActive(true);
			BtnSave.SetActive(true);

			BtnRecText.text = "PAUSE";
		}

		void OnPauseRecord ()
		{
			print("OnPauseRecord");

			BtnRec.SetActive(true);
			BtnReset.SetActive(true);
			BtnSave.SetActive(true);

			BtnRecText.text = "Continue to record";
		}

		void OnSavedGIFEvent (SaveState saveState)
		{
			print("OnSavedGIFEvent - state = " + saveState.ToString());

			if(saveState == SaveState.Done)
			{
				BtnRec.SetActive(false);
				BtnReset.SetActive(false);
				BtnSave.SetActive(false);
			}
			else if(saveState == SaveState.Saving)
			{
				BtnRec.SetActive(false);
				BtnReset.SetActive(false);
				BtnSave.SetActive(false);
			}
		}

		void OnResetCurrentRecord ()
		{	
			print("OnResetCurrentRecord");

			BtnRec.SetActive(true);
			BtnReset.SetActive(false);
			BtnSave.SetActive(false);

			firstStart = true;

			BtnRecText.text = "Start Record";
		}

		void OnShareGIFEvent ()
		{
			print("OnShareGIFEvent");

			Record.DOReset();

			BtnRec.SetActive(true);
			BtnReset.SetActive(false);
			BtnSave.SetActive(false);

			firstStart = true;

			BtnRecText.text = "Start Record";
		}
	}
}