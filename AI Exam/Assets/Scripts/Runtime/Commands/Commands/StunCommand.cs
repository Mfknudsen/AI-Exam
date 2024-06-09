#region Libraries

using System.Collections;
using Runtime.Soccer.Player;
using UnityEngine;

#endregion

namespace Runtime.Commands.Commands
{
    public class StunCommand : TargetCommandObject
    {
        protected override IEnumerator Command()
        {
            PlayerController controller = this.transform.parent.GetComponent<PlayerController>();
            controller.enabled = false;
            Rigidbody rb = this.transform.parent.GetComponent<Rigidbody>();

            while (Vector3.Distance(this.transform.position, this.target.position) > 1)
            {
                rb.AddForce((this.target.position - this.transform.position).normalized * controller.GetSpeed() *
                            Time.deltaTime);
            }

            this.target.GetComponent<PlayerController>().enabled = false;

            controller.enabled = true;

            yield return new WaitForSeconds(1);

            this.target.GetComponent<PlayerController>().enabled = true;
        }
    }
}