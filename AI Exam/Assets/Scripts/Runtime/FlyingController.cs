#region Libraries

using UnityEngine;

#endregion

namespace Runtime
{
    public class FlyingController : MonoBehaviour
    {
        #region Values

        [SerializeField, Min(0.1f)] private float moveSpeed, rotationSpeed;

        private Vector2 turn;

        private bool active;

        #endregion

        #region Update

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                this.active = !this.active;

                if (this.active)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }

            if (!this.active)
                return;

            Vector3 moveDirection = (this.transform.right * Input.GetAxis("Horizontal") +
                                     this.transform.forward * Input.GetAxis("Vertical") +
                                     Vector3.up * ((Input.GetKey(KeyCode.Space) ? 1 : 0) -
                                                   (Input.GetKey(KeyCode.LeftControl) ? 1 : 0))).normalized;

            this.transform.position += moveDirection *
                                       ((Input.GetKey(KeyCode.LeftShift) ? 3 : 1) * this.moveSpeed * Time.deltaTime);

            this.turn.x += Input.GetAxis("Mouse X") * this.rotationSpeed;
            this.turn.y += Input.GetAxis("Mouse Y") * this.rotationSpeed;

            this.transform.localRotation = Quaternion.Euler(-this.turn.y, this.turn.x, 0);
        }

        #endregion
    }
}