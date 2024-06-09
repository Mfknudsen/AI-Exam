#region Libraries

using System.Collections;
using UnityEngine;

#endregion

namespace Runtime.Commands.Commands
{
    public class ResumeGameCommand : CommandObject
    {
        protected override IEnumerator Command()
        {
            foreach (Transform t in this.transform.parent.parent)
            {
                Rigidbody controller = t.GetComponent<Rigidbody>();
                if (controller != null)
                    controller.isKinematic = false;
                
            }

            yield break;
        }
    }
}