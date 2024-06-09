#region Libraries

using Runtime.Soccer.Player;
using UnityEngine;

#endregion

namespace Runtime.Soccer
{
    public class SoccerBallController : MonoBehaviour
    {
        public GameObject area;
        [HideInInspector] public FieldEnvironment envController;
        public string purpleGoalTag; //will be used to check if collided with purple goal
        public string blueGoalTag; //will be used to check if collided with blue goal

        private void Start() =>
            this.envController = this.area?.GetComponent<FieldEnvironment>();

        private void OnCollisionEnter(Collision col)
        {
            if (this.envController == null) return;

            if (col.gameObject.CompareTag(this.purpleGoalTag)) //ball touched purple goal
                this.envController.GoalTouched(Team.Blue);

            if (col.gameObject.CompareTag(this.blueGoalTag)) //ball touched blue goal
                this.envController.GoalTouched(Team.Purple);
        }
    }
}