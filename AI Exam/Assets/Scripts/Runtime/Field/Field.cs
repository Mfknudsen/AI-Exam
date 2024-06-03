#region Libraries

using System;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Runtime.Field
{
    public sealed class Field : MonoBehaviour
    {
        #region Values

        [SerializeField] private Team team1, team2;

        [SerializeField] private Transform ball;

        private int team1Score, team2Score;

        private UnityEvent<Team> onGameEnd;

        private Vector3 ballStartPosition;
        
        #endregion

        #region Build In States

        private void Start()
        {
            this.ballStartPosition = this.ball.transform.position;
        }

        #endregion

        #region In

        public void Subscribe(UnityAction<Team> action)
        {
            this.onGameEnd ??= new UnityEvent<Team>();

            this.onGameEnd.AddListener(action);
        }

        public void Unsubscribe(UnityAction<Team> action) => this.onGameEnd.RemoveListener(action);

        public void AddGoal(Team team)
        {
            if (team == this.team1)
                this.team1Score++;
            else
                this.team2Score++;

            if (this.team1Score == 3)
                this.onGameEnd.Invoke(this.team1);
            else if (this.team2Score == 3)
                this.onGameEnd.Invoke(this.team2);
        }

        public void EndEpisode()
        {
            this.team1Score = 0;
            this.team2Score = 0;

            this.ball.transform.position = this.ballStartPosition;

            this.team1.EndEpisodeForPlayers();
            this.team2.EndEpisodeForPlayers();
        }

        #endregion
    }
}