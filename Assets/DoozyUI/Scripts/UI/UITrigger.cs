// Copyright (c) 2015 - 2016 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace DoozyUI
{
    [AddComponentMenu("DoozyUI/UI Trigger", 3)]
    public class UITrigger : MonoBehaviour
    {
        #region Context Menu Methods

#if UNITY_EDITOR
        [MenuItem("DoozyUI/Components/UI Trigger", false, 3)]
        [MenuItem("GameObject/DoozyUI/UI Trigger", false, 3)]
        private static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            if (GameObject.Find("UIManager") == null)
            {
                Debug.LogError(
                    "[DoozyUI] The DoozyUI system was not found in the scene. Please add it before trying to create a UI Trigger.");
                return;
            }
            var go = new GameObject("New UITrigger");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            go.AddComponent<UITrigger>();
            Selection.activeObject = go;
        }
#endif

        #endregion

        private void OnEnable()
        {
            if (triggerOnGameEvent)
            {
                if (dispatchAll)
                    gameEvent = UIManager.DISPATCH_ALL;
                else if (string.IsNullOrEmpty(gameEvent))
                    Debug.Log("[DoozyUI] The UITrigger on [" + gameObject.name +
                              "] gameObject is disabled. It will not trigger anything because you didn't enter a game event for it to listen for.");

                UIManager.RegisterUiTrigger(this, UIManager.EventType.GameEvent);
            }
            else if (triggerOnButtonClick)
            {
                if (dispatchAll)
                    buttonName = UIManager.DISPATCH_ALL;
                else if (buttonName.Equals(UIManager.DEFAULT_BUTTON_NAME))
                    Debug.Log("[DoozyUI] The UITrigger on [" + gameObject.name +
                              "] gameObject is disabled. It will not trigger anything because you didn't select a button name for it to listen for.");
                UIManager.RegisterUiTrigger(this, UIManager.EventType.ButtonClick);
            }
            else
            {
                Debug.Log("[DoozyUI] The UITrigger on [" + gameObject.name +
                          "] gameObject is disabled. It will not trigger anything because you didn't select if the trigger should listen for game events or button clicks.");
            }
        }

        private void OnDisable()
        {
            if (triggerOnGameEvent)
            {
                if (dispatchAll)
                    gameEvent = UIManager.DISPATCH_ALL;
                UIManager.UnregisterUiTrigger(this, UIManager.EventType.GameEvent);
            }
            else
            {
                if (dispatchAll)
                    buttonName = UIManager.DISPATCH_ALL;
                UIManager.UnregisterUiTrigger(this, UIManager.EventType.ButtonClick);
            }
        }

        /// <summary>
        ///     Guess what? This triggers the trigger :)
        /// </summary>
        public void TriggerTheTrigger(string triggerValue)
        {
            if (triggerOnGameEvent)
            {
                if (gameEvent.Equals(triggerValue) || dispatchAll)
                {
                    onTriggerEvent.Invoke(triggerValue);

                    if (gameEvents != null && gameEvents.Count > 0)
                        UIManager.SendGameEvents(gameEvents);
                }
            }
            else if (triggerOnButtonClick)
            {
                if (buttonName.Equals(triggerValue) || dispatchAll)
                {
                    onTriggerEvent.Invoke(triggerValue);

                    if (gameEvents != null && gameEvents.Count > 0)
                        UIManager.SendGameEvents(gameEvents);
                }
            }
        }

        #region Internal Classes - TriggerEvent

        [Serializable]
        public class TriggerEvent : UnityEvent<string>
        {
        }

        #endregion

        #region Public Variables

        [HideInInspector] public bool showHelp;

        public bool triggerOnGameEvent;
        public string gameEvent = string.Empty;


        public bool triggerOnButtonClick;
        public string buttonName = string.Empty;

        public bool dispatchAll;

        [SerializeField] private TriggerEvent onTriggerEvent = new TriggerEvent();

        public List<string> gameEvents;

        #endregion
    }
}