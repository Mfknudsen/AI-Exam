#region Libraries

using Runtime.Soccer.Player;

#endregion

namespace Runtime.Commands.Commands
{
    public class SetAsStrikerCommand : CommandObject
    {
        private void Start()
        {
            this.transform.parent.GetComponent<PlayerController>().teamRole = PlayerController.TeamRole.Striker;
            Destroy(this.gameObject);
        }
    }
}