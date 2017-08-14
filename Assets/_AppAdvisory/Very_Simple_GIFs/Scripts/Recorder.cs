/*
 * Copyright (c) 2015 Thomas Hourdel
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 *    1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 
 *    2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 
 *    3. This notice may not be removed or altered from any source
 *    distribution.
 */





using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ThreadPriority = System.Threading.ThreadPriority;

#if VS_UI
using AppAdvisory.VSUI;
#endif

namespace AppAdvisory.VSGIF
{
	[AddComponentMenu("Miscellaneous/Moments Recorder")]
	[RequireComponent(typeof(Camera)), DisallowMultipleComponent]
	public sealed class Recorder : MonoBehaviour
	{

		void DisplaySpriteGIF(bool _activate)
		{
			gifElement.DisplaySpriteGIF(_activate);
		}

		public GIFElement gifElement;
//		public GIFElement gifElement
//		{
//			get
//			{
//				if(_gifElement == null)
//					_gifElement = GameObject.FindObjectOfType<GifEncoder>(true);
//
//				return _gifElement;
//			}
//
//			set
//			{
//				_gifElement = value;
//			}
//		}

	
		

		void Awake()
		{
//			gifElement = FindObjectOfType<GIFElement>();
		}

//		void OnDestroy()
//		{
//			FlushMemory();
//		}

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (gifElement.State != RecorderState.Recording)
			{
				Graphics.Blit(source, destination);
				return;
			}

			gifElement.m_Time += Time.unscaledDeltaTime;

			if (gifElement.m_Time >=gifElement. m_TimePerFrame)
			{
				// Limit the amount of frames stored in memory
				if (gifElement.m_Frames.Count >= gifElement.m_MaxFrameCount)
					gifElement.m_RecycledRenderTexture = gifElement.m_Frames.Dequeue();

				gifElement.m_Time -= gifElement.m_TimePerFrame;

				// Frame data
				RenderTexture rt = gifElement.m_RecycledRenderTexture;
				gifElement.m_RecycledRenderTexture = null;

				if (rt == null)
				{
					rt = new RenderTexture(gifElement.gifSettings.width, gifElement.m_Height, 0, RenderTextureFormat.ARGB32);
					rt.wrapMode = TextureWrapMode.Clamp;
					rt.filterMode = FilterMode.Bilinear;
					rt.anisoLevel = 0;
				}

				Graphics.Blit(source, rt);
				gifElement.m_Frames.Enqueue(rt);
			}

			Graphics.Blit(source, destination);
		}


	}

}


