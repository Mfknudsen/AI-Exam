#region Libraries

using System;
using Runtime.Player;
using UnityEngine;

#endregion

namespace Runtime.Field
{
    public sealed class Team : MonoBehaviour
    {
        #region Values

        private PlayerController[] playersInTeam;

        #endregion

        #region Build In States

        private void Start()
        {
            this.playersInTeam = this.GetComponentsInChildren<PlayerController>();
            this.transform.parent.parent.GetComponent<Field>().Subscribe(this.OnGameEnd);
        }

        private void OnDisable()
        {
            this.transform.parent.parent.GetComponent<Field>().Unsubscribe(this.OnGameEnd);
        }

        #endregion

        #region Internal

        private void OnGameEnd(Team team)
        {
            if (team != this) return;
            
            foreach (PlayerController playerController in this.GetComponentsInChildren<PlayerController>())
                playerController.AddGoalReward();
        }

        #endregion
    }
}