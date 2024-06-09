#region Libraries

using Runtime.Soccer.Player;

#endregion

namespace Runtime.Commands.Commands
{
    public class SetAsGoalieCommand : CommandObject
    {
        private void Start()
        {
            this.transform.parent.GetComponent<PlayerController>().teamRole = PlayerController.TeamRole.Goalie;
            Destroy(this.gameObject);
        }
    }
}