#region Libraries

using UnityEngine;

#endregion

namespace Runtime.Commands
{
    public abstract class TargetCommandObject : CommandObject
    {
        protected Transform target;

        public void SetTarget(Transform set) => this.target = set;
    }
}