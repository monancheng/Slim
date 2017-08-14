
/***********************************************************************************************************
 * Produced by App Advisory - http://app-advisory.com
 * Facebook: https://facebook.com/appadvisory
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch
 ***********************************************************************************************************/




using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using MyThreadPriority = System.Threading.ThreadPriority;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AppAdvisory.VSGIF
{
	public class GIFSettings : ScriptableObject 
	{

		public string giphyKey = "dc6zaTOxFJmzC";

		[Range(8, 4096)]
		public int width = 256;
		[Range(1, 30)]
		public int framePerSecond = 15;
		[Range(-1, 10)]
		public int repeat = 0;
		[Range(1, 100)]
		public int quality = 70;
		[Range(0.1f, 10f)]
		public float bufferSize = 3f;
		public MyThreadPriority WorkerPriority = MyThreadPriority.Highest;

		public Texture2D watermark;


		public bool isGIFFoldoutOpened = false;

		#region EDITOR

		public static readonly string PATH = "Assets/_AppAdvisory/Very_Simple_GIFs/";
		public static readonly string NAME = "GIFSettings";

		private static string PathToAsset 
		{
			get 
			{
				return PATH + NAME + ".asset";
			}
		}

		#if UNITY_EDITOR

		[MenuItem("Assets/Create/AppAdvisory/GIFSettings")]
		public static void CreateGIFSettings()
		{
			GIFSettings asset = ScriptableObject.CreateInstance<GIFSettings>();

			AssetDatabase.CreateAsset(asset, PathToAsset);
			AssetDatabase.SaveAssets();

			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset;
		}

		#endif

		#endregion
	}

}