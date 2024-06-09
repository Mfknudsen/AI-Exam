#region Libraries

using Runtime.Commands;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;

#endregion

namespace Runtime.Soccer.Player
{
    public enum Team
    {
        Blue = 0,
        Purple = 1
    }

    public sealed class PlayerController : Agent
    {
        // Note that the detectable tags are different for the blue and purple teams. The order is
        // * ball
        // * own goal
        // * opposing goal
        // * wall
        // * own teammate
        // * opposing player

        public enum TeamRole
        {
            Generic,
            Striker,
            Goalie,
        }

        [HideInInspector] public Team team;

        public Transform ownGoal, otherGoal;

        private float mKickPower;

        // The coefficient for the reward for colliding with a ball. Set using curriculum.
        private float mBallTouch;
        public TeamRole teamRole;

        private const float KPower = 2000f;
        private float mExistential;
        private float mLateralSpeed;
        private float mForwardSpeed;

        public Transform ball;

        [HideInInspector] public Rigidbody agentRb;
        private SoccerSettings mSoccerSettings;
        private BehaviorParameters mBehaviorParameters;
        public Vector3 initialPos;
        public float rotSign;

        private bool inCorner;

        public override void Initialize()
        {
            FieldEnvironment envController = this.GetComponentInParent<FieldEnvironment>();
            if (envController != null)
            {
                this.mExistential = .5f / envController.maxEnvironmentSteps;
            }
            else
            {
                this.mExistential = 1f / this.MaxStep;
            }

            this.mBehaviorParameters = this.gameObject.GetComponent<BehaviorParameters>();
            if (this.mBehaviorParameters.TeamId == (int)Team.Blue)
            {
                this.team = Team.Blue;
                //this.initialPos = new Vector3(this.transform.position.x - 5f, .5f, this.transform.position.z);
                this.rotSign = 1f;
            }
            else
            {
                this.team = Team.Purple;
                //this.initialPos = new Vector3(this.transform.position.x + 5f, .5f, this.transform.position.z);
                this.rotSign = -1f;
            }

            this.initialPos = this.transform.position;

            if (this.teamRole == TeamRole.Goalie)
            {
                this.mLateralSpeed = 1.0f;
                this.mForwardSpeed = 1.0f;
            }
            else if (this.teamRole == TeamRole.Striker)
            {
                this.mLateralSpeed = 0.3f;
                this.mForwardSpeed = 1.3f;
            }
            else
            {
                this.mLateralSpeed = 0.3f;
                this.mForwardSpeed = 1.0f;
            }

            this.mSoccerSettings = FindObjectOfType<SoccerSettings>();
            this.agentRb = this.GetComponent<Rigidbody>();
            this.agentRb.maxAngularVelocity = 500;
        }

        private void MoveAgent(ActionSegment<int> act)
        {
            Vector3 rotateDir = Vector3.zero;

            this.mKickPower = 0f;

            int forwardAxis = act[0];
            int rightAxis = act[1];
            int rotateAxis = act[2];

            if (forwardAxis == 1)
                this.mKickPower = 1f;

            Vector3 dirToGo = this.transform.right * (rightAxis - 1) * this.mLateralSpeed +
                              this.transform.forward * (forwardAxis - 1) * this.mForwardSpeed;

            rotateDir = rotateAxis switch
            {
                1 => this.transform.up * -1f,
                2 => this.transform.up * 1f,
                _ => rotateDir
            };

            this.transform.Rotate(rotateDir, Time.deltaTime * 100f);
            this.agentRb.AddForce(dirToGo * this.mSoccerSettings.agentRunSpeed, ForceMode.VelocityChange);
            //this.agentRb.MovePosition(this.transform.position + dirToGo * this.mSoccerSettings.agentRunSpeed * Time.deltaTime);
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            if (this.teamRole == TeamRole.Goalie)
            {
                // Existential bonus for Goalies.
                if (Vector3.Distance(this.transform.position, this.ownGoal.position) < 5 && !this.inCorner)
                    this.AddReward(this.mExistential);
            }
            else if (this.teamRole == TeamRole.Striker)
            {
                // Existential penalty for Strikers
                this.AddReward(-this.mExistential);
            }

            this.MoveAgent(actionBuffers.DiscreteActions);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<int> discreteActionsOut = actionsOut.DiscreteActions;
            //forward
            if (Input.GetKey(KeyCode.W))
            {
                discreteActionsOut[0] = 1;
            }

            if (Input.GetKey(KeyCode.S))
            {
                discreteActionsOut[0] = 2;
            }

            //rotate
            if (Input.GetKey(KeyCode.A))
            {
                discreteActionsOut[2] = 1;
            }

            if (Input.GetKey(KeyCode.D))
            {
                discreteActionsOut[2] = 2;
            }

            //right
            if (Input.GetKey(KeyCode.E))
            {
                discreteActionsOut[1] = 1;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                discreteActionsOut[1] = 2;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((this.gameObject.CompareTag("blueAgent") && other.CompareTag("cornerBlue")) ||
                (this.gameObject.CompareTag("purpleAgent") && other.CompareTag("cornerPurple")))
                this.inCorner = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if ((this.gameObject.CompareTag("blueAgent") && other.CompareTag("cornerBlue")) ||
                (this.gameObject.CompareTag("purpleAgent") && other.CompareTag("cornerPurple")))
                this.inCorner = false;
        }

        /// <summary>
        /// Used to provide a "kick" to the ball.
        /// </summary>
        private void OnCollisionEnter(Collision c)
        {
            if (c.gameObject.CompareTag("wall"))
            {
                this.AddReward(-.1f);
                return;
            }

            if (!c.gameObject.CompareTag("ball")) return;

            float force = KPower * this.mKickPower;
            if (this.teamRole == TeamRole.Goalie)
            {
                force = KPower;
            }

            switch (this.teamRole)
            {
                case TeamRole.Striker when !this.inCorner:
                    this.AddReward(this.mBallTouch);
                    break;
                case TeamRole.Goalie when
                    Vector3.Distance(this.transform.position, this.ownGoal.position) < 5:
                    this.AddReward(this.mBallTouch);
                    break;
            }

            Vector3 dir = c.contacts[0].point - this.transform.position;
            dir = dir.normalized;
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
        }

        public override void OnEpisodeBegin()
        {
            this.mBallTouch = .5f;

            CommandObject[] c = this.GetComponentsInChildren<CommandObject>();
            for (int i = c.Length - 1; i >= 0; i--)
                Destroy(c[i].gameObject);
        }

        public float GetSpeed() => this.mSoccerSettings.agentRunSpeed;
    }
}