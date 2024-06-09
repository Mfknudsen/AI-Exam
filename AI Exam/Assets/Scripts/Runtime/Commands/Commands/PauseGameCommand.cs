#region Libraries

using UnityEngine;

#endregion

namespace Runtime.Commands.Commands
{
    public class PauseGameCommand : CommandObject
    {
        private void Start()
        {
            foreach (Transform t in this.transform.parent.parent)
            {
                if (!(t.gameObject.CompareTag("blueAgent") || t.gameObject.CompareTag("purpleAgent") ||
                      t.gameObject.CompareTag("ball")))
                    continue;

                Rigidbody controller = t.GetComponent<Rigidbody>();
                if (controller != null)
                    controller.isKinematic = true;
            }

            Destroy(this.gameObject);
        }
    }
}