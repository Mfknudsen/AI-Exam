#region Libraries

using System;
using Runtime.Field;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

namespace Runtime.Player
{
    public class PlayerController : Agent
    {
        #region Values

#if UNITY_EDITOR
        [SerializeField] private bool syntheticCameraInput;
#endif

        private Rigidbody rb;

        private Vector3 moveDirection;
        private int rotateDirection;

        [SerializeField] private float moveSpeed = 5, rotateSpeed = 10;

        [SerializeField] private LayerMask layerMask;

        [SerializeField] private Transform ball, goal;

        private bool ballInSight;

        private Vector3 predictedBallPosition;

        private float previousBallDistance = 100, ballGoalDistance = 100, ballSightAngle;

        private Vector3 startPosition;

        private Quaternion startRotation;

        #endregion

        #region Build In States

        private void Start()
        {
            this.rb = this.GetComponent<Rigidbody>();

            if (this.ball is null)
                throw new Exception($"Ball missing for: {this.gameObject.name}");
            if (this.goal is null)
                throw new Exception($"Goal missing for: {this.gameObject.name}");

            this.startPosition = this.transform.position;
            this.startRotation = this.transform.rotation;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (this.syntheticCameraInput)
            {
                this.BallInSight();
                this.BallPosition();
            }
#endif

            this.rb.MovePosition(this.transform.position + this.moveDirection * (this.moveSpeed * Time.deltaTime));
            this.transform.Rotate(Vector3.up, this.rotateDirection * this.rotateSpeed * Time.deltaTime);

            this.UpdateRewards();
        }

        public override void OnEpisodeBegin()
        {
            this.transform.position = this.startPosition;
            this.transform.rotation = this.startRotation;
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            ActionSegment<float> c = actions.ContinuousActions;
            this.moveDirection = new Vector3(c[0], 0f, c[1]).normalized;
            this.rotateDirection = actions.DiscreteActions[0] - 1;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(this.transform.position);
            sensor.AddObservation(this.ballInSight);
            sensor.AddObservation(this.predictedBallPosition);
            sensor.AddObservation(this.goal.transform.position);
            sensor.AddObservation(this.previousBallDistance);
            sensor.AddObservation(this.ballSightAngle);
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log(other.gameObject.layer.ToString());
            if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                this.AddReward(-50);
                this.transform.parent.GetComponent<Team>()?.EndEpisode();
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Ball"))
                this.AddReward(50);
        }

        #endregion

        #region Setters

        public void SetBallInSight(bool set) => this.ballInSight = set;

        public void SetPredictedBallPosition(Vector3 set) => this.predictedBallPosition = set.normalized;

        #endregion

        #region In

        public void AddGoalReward()
        {
            this.AddReward(50);
        }

        #endregion

        #region Internal

#if UNITY_EDITOR
        private void BallInSight()
        {
            Vector2 playerPos = new Vector2(this.transform.position.x, this.transform.position.z),
                ballPos = new Vector2(this.ball.transform.position.x, this.ball.transform.position.z);
            this.ballInSight = Vector2.Angle(playerPos, ballPos) < 40f;
        }

        private void BallPosition()
        {
            if (!this.ballInSight)
                return;

            this.predictedBallPosition = this.ball.position +
                                         new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f),
                                             Random.Range(-1f, 1f));
        }
#endif

        private void UpdateRewards()
        {
            this.SetReward(this.ballInSight ? 1 : -1);
            
            float dist = Vector3.Distance(this.predictedBallPosition, this.transform.position);
            this.AddReward(dist < this.previousBallDistance ? 1 : -1);
            this.previousBallDistance = dist;

            dist = Vector3.Distance(this.predictedBallPosition, this.goal.transform.position);
            this.AddReward(dist < this.ballGoalDistance ? 1 : -1);
            this.ballGoalDistance = dist;

            float angle = Vector3.Angle(this.transform.forward, this.predictedBallPosition - this.transform.position);
            this.AddReward(angle < this.ballSightAngle ? 1 : -1);
            this.ballSightAngle = angle;
        }

        #endregion
    }
}