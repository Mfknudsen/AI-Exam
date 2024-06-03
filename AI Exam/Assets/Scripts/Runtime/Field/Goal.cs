#region Libraries

using UnityEngine;

#endregion

namespace Runtime.Field
{
    public sealed class Goal : MonoBehaviour
    {
        #region Values

        [SerializeField] private Team team;

        private Field field;

        #endregion

        #region Build In States

        private void Start()
        {
            this.field = this.transform.parent.GetComponent<Field>();
        }

        #endregion

        #region Internal

        private void AddGoalScore()
        {
            this.field.AddGoal(this.team);
        }

        #endregion
    }
}