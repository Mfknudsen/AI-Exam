#region Libraries

using System.Collections;
using Runtime.Soccer.Player;
using UnityEngine;

#endregion

namespace Runtime.Commands.Commands
{
    public class EmoteCommand : CommandObject
    {
        public int jumpCount;

        protected override IEnumerator Command()
        {
            PlayerController controller = this.transform.parent.GetComponent<PlayerController>();
            controller.enabled = false;

            float y = this.transform.position.y;
            int count = 0;
            while (count < this.jumpCount)
            {
                float yForce = 5;
                this.transform.parent.position += Vector3.up * yForce * Time.deltaTime;
                yield return null;
                while (this.transform.parent.position.y > y)
                {
                    yForce -= 9.81f * Time.deltaTime;
                    this.transform.parent.position += Vector3.up * yForce * Time.deltaTime;
                    this.transform.parent.Rotate(Vector3.up, 500 * Time.deltaTime);
                    Debug.Log(yForce);
                    yield return null;
                }

                this.transform.position = new Vector3(this.transform.position.x, y, this.transform.position.z);

                count++;

                yield return new WaitForSeconds(.5f);
            }

            controller.enabled = true;
        }
    }
}