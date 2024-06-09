#region Libraries

using System.Collections;
using Runtime.Soccer.Player;

#endregion

namespace Runtime.Commands.Commands
{
    public class SetAsStrikerCommand : CommandObject
    {
        protected override IEnumerator Command()
        {
            this.transform.parent.GetComponent<PlayerController>().teamRole = PlayerController.TeamRole.Striker;
            yield break;
        }
    }
}