#region Libraries

using UnityEngine;

#endregion

namespace Runtime.Player
{
    public sealed class CameraController : MonoBehaviour
    {
        #region Build In States

        private void OnEnable()
        {
            CameraUpdater.AddController(this);
        }

        private void OnDisable()
        {
            CameraUpdater.RemoveController(this);
        }

        #endregion

        #region In

        public void UpdateCamera()
        {
        }

        #endregion
    }
}