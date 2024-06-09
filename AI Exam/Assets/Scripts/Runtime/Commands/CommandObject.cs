#region Libraries

using System.Collections;
using UnityEngine;

#endregion

namespace Runtime.Commands
{
    public abstract class CommandObject : MonoBehaviour
    {
        public string commandText;

        private IEnumerator Start()
        {
            yield return this.Command();

            Destroy(this.gameObject);
        }

        protected abstract IEnumerator Command();
    }
}