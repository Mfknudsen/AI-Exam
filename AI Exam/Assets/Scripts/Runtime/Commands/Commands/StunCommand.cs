#region Libraries

using Runtime.Soccer.Player;
using UnityEngine;

#endregion

namespace Runtime.Commands.Commands
{
    public class StunCommand : TargetCommandObject
    {
        private Rigidbody rb;

        private float radius;

        private PlayerController controller;

        private bool stunned;

        private float t = 5;

        private void Start()
        {
            this.controller = this.transform.parent.GetComponent<PlayerController>();
            this.rb = this.controller.GetComponent<Rigidbody>();
            this.controller.enabled = false;

            this.radius = this.transform.parent.GetComponent<CapsuleCollider>().radius;
        }

        private void Update()
        {
            if (!this.stunned && Vector3.Distance(this.controller.transform.position, this.target.position) >
                this.radius * 2.5f)
            {
                this.rb.AddForce(
                    (this.target.position - this.controller.transform.position).normalized *
                    this.controller.GetSpeed() / 5, ForceMode.VelocityChange);

                return;
            }

            this.stunned = true;
            this.target.GetComponent<PlayerController>().enabled = false;
            this.controller.enabled = true;

            if (this.t > 0)
            {
                this.t -= Time.deltaTime;

                return;
            }

            Destroy(this.gameObject);
        }

        private void OnDestroy()
        {
            this.target.GetComponent<PlayerController>().enabled = true;
            this.controller.enabled = true;
        }
    }
}