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
                if (t.GetComponent<Rigidbody>() is { } controller)
                    controller.isKinematic = false;
            }

            yield break;
        }
    }
}