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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name.Equals("Ball"))
                this.field.AddGoal(this.team);
        }

        #endregion
    }
}