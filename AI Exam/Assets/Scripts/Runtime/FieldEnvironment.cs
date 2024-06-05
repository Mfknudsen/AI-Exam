using System.Collections.Generic;
using Runtime.Player;
using Unity.MLAgents;
using UnityEngine;

namespace Runtime
{
    public class FieldEnvironment : MonoBehaviour
    {
        [System.Serializable]
        public class PlayerInfo
        {
            public PlayerController agent;
            [HideInInspector] public Vector3 startingPos;
            [HideInInspector] public Quaternion startingRot;
            [HideInInspector] public Rigidbody rb;
        }


        /// <summary>
        /// Max Academy steps before this platform resets
        /// </summary>
        /// <returns></returns>
        [Tooltip("Max Environment Steps")] public int maxEnvironmentSteps = 25000;

        /// <summary>
        /// The area bounds.
        /// </summary>
        /// <summary>
        /// We will be changing the ground material based on success/failure
        /// </summary>
        public GameObject ball;

        [HideInInspector] public Rigidbody ballRb;

        private Vector3 mBallStartingPos;

        //List of Agents On Platform
        public List<PlayerInfo> agentsList = new List<PlayerInfo>();

        private SoccerSettings mSoccerSettings;

        private SimpleMultiAgentGroup mBlueAgentGroup;
        private SimpleMultiAgentGroup mPurpleAgentGroup;

        private int mResetTimer;

        private void Start()
        {
            this.mSoccerSettings = FindObjectOfType<SoccerSettings>();
            // Initialize TeamManager
            this.mBlueAgentGroup = new SimpleMultiAgentGroup();
            this.mPurpleAgentGroup = new SimpleMultiAgentGroup();
            this.ballRb = this.ball.GetComponent<Rigidbody>();
            this.mBallStartingPos = new Vector3(this.ball.transform.position.x, this.ball.transform.position.y,
                this.ball.transform.position.z);
            foreach (PlayerInfo item in this.agentsList)
            {
                item.startingPos = item.agent.transform.position;
                item.startingRot = item.agent.transform.rotation;
                item.rb = item.agent.GetComponent<Rigidbody>();
                if (item.agent.team == Team.Blue)
                {
                    this.mBlueAgentGroup.RegisterAgent(item.agent);
                }
                else
                {
                    this.mPurpleAgentGroup.RegisterAgent(item.agent);
                }
            }

            this.ResetScene();
        }

        private void FixedUpdate()
        {
            this.mResetTimer += 1;
            if (this.mResetTimer >= this.maxEnvironmentSteps && this.maxEnvironmentSteps > 0)
            {
                this.mBlueAgentGroup.GroupEpisodeInterrupted();
                this.mPurpleAgentGroup.GroupEpisodeInterrupted();
                this.ResetScene();
            }
        }


        private void ResetBall()
        {
            float randomPosX = Random.Range(-2.5f, 2.5f);
            float randomPosZ = Random.Range(-2.5f, 2.5f);

            this.ball.transform.position = this.mBallStartingPos + new Vector3(randomPosX, 0f, randomPosZ);
            this.ballRb.velocity = Vector3.zero;
            this.ballRb.angularVelocity = Vector3.zero;
        }

        public void GoalTouched(Team scoredTeam)
        {
            if (scoredTeam == Team.Blue)
            {
                this.mBlueAgentGroup.AddGroupReward(1 - (float)this.mResetTimer / this.maxEnvironmentSteps);
                this.mPurpleAgentGroup.AddGroupReward(-1);
            }
            else
            {
                this.mPurpleAgentGroup.AddGroupReward(1 - (float)this.mResetTimer / this.maxEnvironmentSteps);
                this.mBlueAgentGroup.AddGroupReward(-1);
            }

            this.mPurpleAgentGroup.EndGroupEpisode();
            this.mBlueAgentGroup.EndGroupEpisode();
            this.ResetScene();
        }


        private void ResetScene()
        {
            this.mResetTimer = 0;

            //Reset Agents
            foreach (PlayerInfo item in this.agentsList)
            {
                float randomPosX = Random.Range(-2f, 2f);
                Vector3 newStartPos = item.agent.initialPos + new Vector3(randomPosX, 0, 0);
                float rot = item.agent.rotSign * Random.Range(80.0f, 100.0f);
                Quaternion newRot = Quaternion.Euler(0, rot, 0);
                item.agent.transform.SetPositionAndRotation(newStartPos, newRot);

                item.rb.velocity = Vector3.zero;
                item.rb.angularVelocity = Vector3.zero;
            }

            //Reset Ball
            this.ResetBall();
        }
    }
}