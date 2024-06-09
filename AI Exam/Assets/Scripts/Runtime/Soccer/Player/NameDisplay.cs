using TMPro;
using UnityEngine;

namespace Runtime.Soccer.Player
{
    public class NameDisplay : MonoBehaviour
    {
        public TextMeshPro textMeshPro;

        private Camera mainCamera;

        private void Start()
        {
            this.textMeshPro.text = this.transform.parent.name;
        }

        private void Update()
        {
            if (this.mainCamera == null)
                this.mainCamera = Camera.main;
            else
                this.transform.LookAt(this.mainCamera.transform.position);
        }
    }
}