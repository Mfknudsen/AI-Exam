#region Libraries

using Unity.MLAgents;
using UnityEngine;

#endregion

namespace Runtime.Soccer
{
    /// <summary>
    /// A helper class for the ML-Agents example scenes to override various
    /// global settings, and restore them afterward.
    /// This can modify some Physics and time-stepping properties, so you
    /// shouldn't copy it into your project unless you know what you're doing.
    /// </summary>
    public class ProjectSettingsOverrides : MonoBehaviour
    {
        // Original values
        private Vector3 mOriginalGravity;
        private float mOriginalFixedDeltaTime;
        private float mOriginalMaximumDeltaTime;
        private int mOriginalSolverIterations;
        private int mOriginalSolverVelocityIterations;
        private bool mOriginalReuseCollisionCallbacks;

        [Tooltip("Increase or decrease the scene gravity. Use ~3x to make things less floaty")]
        public float gravityMultiplier = 1.0f;

        [Header("Advanced physics settings")]
        [Tooltip(
            "The interval in seconds at which physics and other fixed frame rate updates (like MonoBehaviour's FixedUpdate) are performed.")]
        public float fixedDeltaTime = .02f;

        [Tooltip(
            "The maximum time a frame can take. Physics and other fixed frame rate updates (like MonoBehaviour's FixedUpdate) will be performed only for this duration of time per frame.")]
        public float maximumDeltaTime = 1.0f / 3.0f;

        [Tooltip(
            "Determines how accurately Rigidbody joints and collision contacts are resolved. (default 6). Must be positive.")]
        public int solverIterations = 6;

        [Tooltip(
            "Affects how accurately the Rigidbody joints and collision contacts are resolved. (default 1). Must be positive.")]
        public int solverVelocityIterations = 1;

        [Tooltip(
            "Determines whether the garbage collector should reuse only a single instance of a Collision type for all collision callbacks. Reduces Garbage.")]
        public bool reuseCollisionCallbacks = true;

        public void Awake()
        {
            // Save the original values
            this.mOriginalGravity = Physics.gravity;
            this.mOriginalFixedDeltaTime = Time.fixedDeltaTime;
            this.mOriginalMaximumDeltaTime = Time.maximumDeltaTime;
            this.mOriginalSolverIterations = Physics.defaultSolverIterations;
            this.mOriginalSolverVelocityIterations = Physics.defaultSolverVelocityIterations;
            this.mOriginalReuseCollisionCallbacks = Physics.reuseCollisionCallbacks;

            // Override
            Physics.gravity *= this.gravityMultiplier;
            Time.fixedDeltaTime = this.fixedDeltaTime;
            Time.maximumDeltaTime = this.maximumDeltaTime;
            Physics.defaultSolverIterations = this.solverIterations;
            Physics.defaultSolverVelocityIterations = this.solverVelocityIterations;
            Physics.reuseCollisionCallbacks = this.reuseCollisionCallbacks;

            // Make sure the Academy singleton is initialized first, since it will create the SideChannels.
            Academy.Instance.EnvironmentParameters.RegisterCallback("gravity",
                f => { Physics.gravity = new Vector3(0, -f, 0); });
        }

        public void OnDestroy()
        {
            Physics.gravity = this.mOriginalGravity;
            Time.fixedDeltaTime = this.mOriginalFixedDeltaTime;
            Time.maximumDeltaTime = this.mOriginalMaximumDeltaTime;
            Physics.defaultSolverIterations = this.mOriginalSolverIterations;
            Physics.defaultSolverVelocityIterations = this.mOriginalSolverVelocityIterations;
            Physics.reuseCollisionCallbacks = this.mOriginalReuseCollisionCallbacks;
        }
    }
}