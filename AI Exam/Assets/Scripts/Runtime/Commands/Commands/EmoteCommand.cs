#region Libraries

using System;
using Runtime.Soccer.Player;
using UnityEngine;

#endregion

namespace Runtime.Commands.Commands
{
    public class EmoteCommand : CommandObject
    {
        public int jumpCount;

        private PlayerController controller;

        private float y, yForce;
        private int count;

        private void Start()
        {
            this.controller = this.transform.parent.GetComponent<PlayerController>();
            this.controller.enabled = false;

            this.y = this.transform.position.y;
            this.count = 0;
        }

        private void Update()
        {
            if (this.count < this.jumpCount)
            {
                this.transform.parent.position += Vector3.up * (this.yForce * Time.deltaTime);
                if (this.transform.parent.position.y > this.y)
                {
                    this.yForce -= 9.81f * Time.deltaTime;
                    this.transform.parent.position += Vector3.up * (this.yForce * Time.deltaTime);
                    this.transform.parent.Rotate(Vector3.up, 500 * Time.deltaTime);
                    return;
                }

                this.transform.position = new Vector3(this.transform.position.x, this.y, this.transform.position.z);

                this.count++;
                this.yForce = 5;
                return;
            }

            Destroy(this.gameObject);
        }

        private void OnDestroy()
        {
            this.controller.enabled = true;
        }
    }
}