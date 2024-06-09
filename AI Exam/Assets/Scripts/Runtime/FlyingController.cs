#region Libraries

using UnityEngine;

#endregion

namespace Runtime
{
    public class FlyingController : MonoBehaviour
    {
        #region Values

        [SerializeField, Min(0.1f)] private float moveSpeed, rotationSpeed;

        #endregion

        #region Update

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        private void Update()
        {
            Vector3 moveDirection = (this.transform.right * Input.GetAxis("Horizontal") +
                                     this.transform.forward * Input.GetAxis("Vertical") +
                                     Vector3.up * ((Input.GetKey(KeyCode.Space) ? 1 : 0) -
                                                   (Input.GetKey(KeyCode.LeftControl) ? 1 : 0))).normalized;

            this.transform.position += moveDirection *
                                       ((Input.GetKey(KeyCode.LeftShift) ? 3 : 1) * this.moveSpeed * Time.deltaTime);

            this.transform.eulerAngles += new Vector3(
                2.5f * -Input.GetAxis("Mouse Y") * this.rotationSpeed * Time.deltaTime,
                Input.GetAxis("Mouse X") * this.rotationSpeed * Time.deltaTime,
                0f);

            this.transform.eulerAngles = new Vector3(Mathf.Clamp(this.transform.eulerAngles.x, -80, 80), this.transform.eulerAngles.y, 0);
        }

        #endregion
    }
}