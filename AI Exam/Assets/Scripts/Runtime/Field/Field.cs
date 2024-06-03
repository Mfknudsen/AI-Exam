#region Libraries

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Runtime.Field
{
    public sealed class Field : MonoBehaviour
    {
        #region Values

        [SerializeField] private Team team1, team2;

        private int team1Score, team2Score;

        private UnityEvent<Team> onGameEnd;

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

        #endregion
    }
}