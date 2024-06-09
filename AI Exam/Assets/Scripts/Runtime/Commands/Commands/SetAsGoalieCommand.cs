#region Libraries

using System.Collections;
using Runtime.Soccer.Player;

#endregion

namespace Runtime.Commands.Commands
{
    public class SetAsGoalieCommand : CommandObject
    {
        protected override IEnumerator Command()
        {
            this.transform.parent.GetComponent<PlayerController>().teamRole = PlayerController.TeamRole.Goalie;
            yield break;
        }
    }
}