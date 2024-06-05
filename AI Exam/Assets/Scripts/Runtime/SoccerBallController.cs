#region Libraries

using Runtime.Player;
using UnityEngine;

#endregion

namespace Runtime
{
    public class SoccerBallController : MonoBehaviour
    {
        public GameObject area;
        [HideInInspector] public FieldEnvironment envController;
        public string purpleGoalTag; //will be used to check if collided with purple goal
        public string blueGoalTag; //will be used to check if collided with blue goal

        private void Start() => 
            this.envController = this.area.GetComponent<FieldEnvironment>();

        private void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.CompareTag(this.purpleGoalTag)) //ball touched purple goal
                this.envController.GoalTouched(Team.Blue);

            if (col.gameObject.CompareTag(this.blueGoalTag)) //ball touched blue goal
                this.envController.GoalTouched(Team.Purple);
        }
    }
}