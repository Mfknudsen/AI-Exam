#region Libraries

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

#endregion

namespace Runtime.Soccer.Player
{
    public static class CameraUpdater
    {
        #region Values

        private static List<CameraController> _cameras;

        private static int _index;

        private const int MaxUpdatesPerFrame = 200;

        #endregion

        #region In

        public static void AddController(CameraController controller) => _cameras.Add(controller);

        public static void RemoveController(CameraController controller) => _cameras.Remove(controller);

        #endregion

        #region Internal

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
            _cameras = new List<CameraController>();

            PlayerLoopSystem playerLoopSystem = PlayerLoop.GetCurrentPlayerLoop();
            for (int i = 0; i < playerLoopSystem.subSystemList.Length; i++)
            {
                if (playerLoopSystem.subSystemList[i].type == typeof(Update))
                    playerLoopSystem.subSystemList[i].updateDelegate += UpdateLoop;
            }

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnExitPlayMode;
#endif
        }

        private static void OnExitPlayMode(PlayModeStateChange stateChange)
        {
            if (!stateChange.Equals(PlayModeStateChange.ExitingPlayMode))
                return;

            PlayerLoopSystem playerLoopSystem = PlayerLoop.GetCurrentPlayerLoop();
            for (int i = 0; i < playerLoopSystem.subSystemList.Length; i++)
            {
                if (playerLoopSystem.subSystemList[i].type == typeof(Update))
                    playerLoopSystem.subSystemList[i].updateDelegate -= UpdateLoop;
            }
        }

        private static void UpdateLoop()
        {
            if (_cameras.Count <= MaxUpdatesPerFrame)
            {
                foreach (CameraController cameraController in _cameras)
                    cameraController.UpdateCamera();

                return;
            }

            for (int i = 0; i < MaxUpdatesPerFrame; i++)
            {
                int j = _index + i;

                if (j >= _cameras.Count)
                    j -= _cameras.Count;

                _cameras[j].UpdateCamera();
            }
        }

        #endregion
    }
}