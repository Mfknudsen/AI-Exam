#region Libraries

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;

#endregion

namespace Runtime.Player
{
    public enum Team
    {
        Blue = 0,
        Purple = 1
    }

    public sealed class PlayerController : Agent
    {
        // Note that that the detectable tags are different for the blue and purple teams. The order is
        // * ball
        // * own goal
        // * opposing goal
        // * wall
        // * own teammate
        // * opposing player

        public enum Position
        {
            Generic,
            Striker,
            Goalie,
        }

        [HideInInspector] public Team team;

        private float mKickPower;

        // The coefficient for the reward for colliding with a ball. Set using curriculum.
        private float mBallTouch;
        public Position position;

        private const float KPower = 2000f;
        private float mExistential;
        private float mLateralSpeed;
        private float mForwardSpeed;


        [HideInInspector] public Rigidbody agentRb;
        private SoccerSettings mSoccerSettings;
        private BehaviorParameters mBehaviorParameters;
        public Vector3 initialPos;
        public float rotSign;

        private EnvironmentParameters mResetParams;

        public override void Initialize()
        {
            FieldEnvironment envController = this.GetComponentInParent<FieldEnvironment>();
            if (envController != null)
            {
                this.mExistential = 1f / envController.maxEnvironmentSteps;
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

            if (this.position == Position.Goalie)
            {
                this.mLateralSpeed = 1.0f;
                this.mForwardSpeed = 1.0f;
            }
            else if (this.position == Position.Striker)
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

            this.mResetParams = Academy.Instance.EnvironmentParameters;
        }

        private void MoveAgent(ActionSegment<int> act)
        {
            Vector3 dirToGo = Vector3.zero;
            Vector3 rotateDir = Vector3.zero;

            this.mKickPower = 0f;

            int forwardAxis = act[0];
            int rightAxis = act[1];
            int rotateAxis = act[2];

            switch (forwardAxis)
            {
                case 1:
                    dirToGo = this.transform.forward * this.mForwardSpeed;
                    this.mKickPower = 1f;
                    break;
                case 2:
                    dirToGo = this.transform.forward * -this.mForwardSpeed;
                    break;
            }

            switch (rightAxis)
            {
                case 1:
                    dirToGo = this.transform.right * this.mLateralSpeed;
                    break;
                case 2:
                    dirToGo = this.transform.right * -this.mLateralSpeed;
                    break;
            }

            rotateDir = rotateAxis switch
            {
                1 => this.transform.up * -1f,
                2 => this.transform.up * 1f,
                _ => rotateDir
            };

            this.transform.Rotate(rotateDir, Time.deltaTime * 100f);
            //this.agentRb.AddForce(dirToGo * this.mSoccerSettings.agentRunSpeed, ForceMode.VelocityChange);
            this.agentRb.MovePosition(this.transform.position +
                                      dirToGo * this.mSoccerSettings.agentRunSpeed * Time.deltaTime);
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            if (this.position == Position.Goalie)
            {
                // Existential bonus for Goalies.
                this.AddReward(this.mExistential);
            }
            else if (this.position == Position.Striker)
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

        /// <summary>
        /// Used to provide a "kick" to the ball.
        /// </summary>
        private void OnCollisionEnter(Collision c)
        {
            if (!c.gameObject.CompareTag("ball")) return;

            float force = KPower * this.mKickPower;
            if (this.position == Position.Goalie)
            {
                force = KPower;
            }

            this.AddReward(.2f * this.mBallTouch);
            Vector3 dir = c.contacts[0].point - this.transform.position;
            dir = dir.normalized;
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
        }

        public override void OnEpisodeBegin()
        {
            //this.mBallTouch = this.mResetParams.GetWithDefault("ball_touch", 0);
            this.mBallTouch = 1;
        }
    }
}