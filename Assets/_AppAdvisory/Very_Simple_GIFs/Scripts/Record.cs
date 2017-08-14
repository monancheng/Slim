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
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//using UnityEngine;
namespace AppAdvisory.VSGIF
{
	[RequireComponent(typeof(Recorder)), AddComponentMenu("")]
	 public class Record : MonoBehaviour
	{

		void Awake()
		{
			self = this;
		}


		GIFElement gifElement
		{
			get
			{
				return m_Recorder.gifElement;
			}
		}

		#region eventListener

		public delegate void StartRecordEvent();
		public static event StartRecordEvent OnStartRecord;

		public static void DOStartRecordEvent()
		{
			if(OnStartRecord != null)
				OnStartRecord();
		}

		public delegate void PauseRecordEvent();
		public static event PauseRecordEvent OnPauseRecord;

		public static void DOPauseRecordEvent()
		{
			if(OnPauseRecord != null)
				OnPauseRecord();
		}

		public delegate void SavedGIFEvent(SaveState saveState);
		public static event SavedGIFEvent OnSavedGIFEvent;

		public static void DOSaveGIFEvent(SaveState saveState)
		{
			if(OnSavedGIFEvent != null)
				OnSavedGIFEvent(saveState);
		}

		public delegate void ResetCurrentRecordEvent();
		public static event ResetCurrentRecordEvent OnResetCurrentRecord;

		public static void DOResetCurrentRecordEvent()
		{
			if(OnResetCurrentRecord != null)
				OnResetCurrentRecord();
		}

		public delegate void ShareGIFEvent();
		public static event ShareGIFEvent OnShareGIFEvent;

		public static void DOShareGIFEvent()
		{
			if(OnShareGIFEvent != null)
				OnShareGIFEvent();
		}
		#endregion

		#region staticVariablesAndMethods

		static private Record self;

		public static void HideGIFButton()
		{
			self.m_Recorder.gifElement.DisplaySpriteGIF(false);
		}

		public static void ShowGIFButton()
		{
			self.m_Recorder.gifElement.DisplaySpriteGIF(true);
		}

		public static bool HaveAGif()
		{
			if(self.m_Recorder == null || self.m_Recorder.gifElement == null)
				return false;
			
			return self.m_Recorder.gifElement.m_sprite != null && self.m_Recorder.gifElement.m_sprite.Count > 1;
		}
	

		public static void DORec()
		{
			DORec(null);
		}

		public static void DORec(Text text)
		{
			if(self.gifElement.State == RecorderState.PreProcessing)
			{
				if(text != null)
					text.text = "REC";

				var State = self.gifElement.Pause();

				DispatchCallbackRecord(State);

				return;
			}

			if(self.gifElement.State == RecorderState.Recording)
			{
				if(text != null)
					text.text = "REC";

				var State = self.gifElement.Pause();

				DispatchCallbackRecord(State);

				return;
			}

			if(self.gifElement.State == RecorderState.Paused)
			{
				if(text != null)
					text.text = "PAUSE";

				var State = self.DORecord();

				DispatchCallbackRecord(State);

				return;
			}
		}

		static void DispatchCallbackRecord(RecorderState State)
		{
			if (State == RecorderState.Recording)
			{
				DOStartRecordEvent();
			}
			else
			{
				DOPauseRecordEvent();
			}
		}

		public static void DOSave()
		{
			self.gifElement.Save();
			self.m_Progress = 0f;

			DOSaveGIFEvent(SaveState.Saving);
			self.StartCoroutine("OnFileSavedCorout");
		}

		public static void DOReset()
		{
			self.gifElement.StopAnimtextureAndDestroySprite();
			DOResetCurrentRecordEvent();
		}
		public static void DOShare(ShareType shareType)
		{
			print("Record - DOShare");

			self.gifElement.ShareGIF(shareType);

			DOShareGIFEvent();
		}
		#endregion

		#region variable
		public  bool DEBUG = false;
		Recorder m_Recorder;
		float m_Progress = 0f;
		string m_LastFile = "";
		bool m_IsSaving = false;

		public string recorderState
		{
			get
			{
				string s = "Recorder State : " + gifElement.State.ToString();
				return s;
			}
		}

		public string progressReport
		{
			get
			{

				string s = "";

				if (m_IsSaving)
					s = "Progress Report : " + m_Progress.ToString("F2") + "%";

				return s;
			}
		}

		public string lastFileSaved
		{
			get
			{
				string s = "";

				if (!string.IsNullOrEmpty(m_LastFile))
					s = "Last File Saved : " + m_LastFile;

				return s;
			}
		}
		#endregion

		#region methods
		void Start()
		{
			// Get our Recorder instance (there can be only one per camera).
			m_Recorder = GetComponent<Recorder>();

			// If you want to change Recorder settings at runtime, use :
			//m_Recorder.Setup(autoAspect, width, height, fps, bufferSize, repeat, quality);
			// Optional callbacks (see each function for more info).
			gifElement.OnPreProcessingDone = OnProcessingDone;
			gifElement.OnFileSaveProgress = OnFileSaveProgress;
			gifElement.OnFileSaved = OnFileSaved;
		}

		RecorderState DORecord()
		{
			// The Recorder starts paused for performance reasons, call Record() to start
			// saving frames to memory. You can pause it at any time by calling Pause().
			return gifElement.Record();
		}

		void OnProcessingDone()
		{
			// All frames have been extracted and sent to a worker thread for compression !
			// The Recorder is ready to record again, you can call Record() here if you don't
			// want to wait for the file to be compresse and saved.
			// Pre-processing is done in the main thread, but frame compression & file saving
			// has its own thread, so you can save multiple gif at once.

			m_IsSaving = true;
		}

		void OnFileSaveProgress(int id, float percent)
		{
			// This callback is probably not thread safe so use it at your own risks.
			// Percent is in range [0;1] (0 being 0%, 1 being 100%).
			m_Progress = percent * 100f;
		}

		void OnFileSaved(int id, string filepath)
		{
			// Our file has successfully been compressed & written to disk !
			m_LastFile = filepath;

			m_IsSaving = false;

			// Let's start recording again (note that we could do that as soon as pre-processing
			// is done and actually save multiple gifs at once, see OnProcessingDone().
			//		m_Recorder.Record();

			gifElement.workerIsDone = true;

			print("OnFileSaved - id = " + id + " - filepath = " + filepath);

			OnDestroy();
		}

		IEnumerator OnFileSavedCorout()
		{
			while(true)
			{
				if(gifElement.workerIsDone == true)
					break;

				yield return 0;
			}

			DOSaveGIFEvent(SaveState.Done);
		}

		void OnDestroy()
		{
			// Memory is automatically flushed when the Recorder is destroyed or (re)setup,
			// but if for some reason you want to do it manually, just call FlushMemory().
			gifElement.FlushMemory();
		}

		//	#if UNITY_EDITOR
		void OnGUI()
		{
			if(!DEBUG)
				return;

			GUILayout.BeginHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginVertical();

			GUILayout.Space(10f);
			GUILayout.Label("Press [SPACE] to export the buffered frames to a gif file.");
			GUILayout.Label("Recorder State : " + gifElement.State.ToString());

			if (m_IsSaving)
				GUILayout.Label("Progress Report : " + m_Progress.ToString("F2") + "%");

			if (!string.IsNullOrEmpty(m_LastFile))
				GUILayout.Label("Last File Saved : " + m_LastFile);

			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
		//	#endif

		#endregion
	}


	public enum ShareType
	{
		Native,
		Facebook,
		Twitter,
		Whatsapp
	}
}