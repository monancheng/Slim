
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

namespace AppAdvisory.VSGIF
{
	[Serializable]
	public class GiphyJson 
	{
		public GiphyData data;
		public GiphyMeta meta;

//		public static GiphyJson CreateFromJSON(string jsonString)
//		{
//			return JsonUtility.FromJson<GiphyJson>(jsonString);
//		}

	}

	[Serializable]
	public class GiphyData
	{
		public string id;
	}

	[Serializable]
	public class GiphyMeta
	{
		public int status;
		public string msg;
		public string response_id;
	}
}