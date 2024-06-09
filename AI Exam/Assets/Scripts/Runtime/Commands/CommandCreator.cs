#region Libraries

using System.Collections.Generic;
using System.Linq;
using Runtime.Soccer.Player;
using UnityEngine;

#endregion

namespace Runtime.Commands
{
    public class CommandCreator : MonoBehaviour
    {
        private List<Command> commands;

        [SerializeField] private List<GameObject> commandPrefabs;

        private void Start()
        {
            this.commands = new List<Command>();

            for (int i = 0; i < this.transform.parent.childCount; i++)
            {
                GameObject obj = this.transform.parent.GetChild(i).gameObject;
                if (obj.GetComponent<PlayerController>() is { } controller)
                {
                    foreach (GameObject commandPrefab in this.commandPrefabs)
                    {
                        if (!commandPrefab.GetComponent<CommandObject>().commandText.Contains("<Name>"))
                            continue;

                        if (commandPrefab.GetComponent<TargetCommandObject>() == null)
                        {
                            Command command = new Command
                            {
                                CommandText = commandPrefab.GetComponent<CommandObject>().commandText
                                    .Replace("<Name>", controller.gameObject.name),
                                CommandObject = commandPrefab,
                                PlayerTransform = obj.transform
                            };

                            if (this.commands.All(c => c.CommandText != command.CommandText))
                                this.commands.Add(command);
                        }
                        else
                        {
                            for (int j = 0; j < this.transform.parent.childCount; j++)
                            {
                                if (i == j)
                                    continue;

                                if (this.transform.parent.GetChild(j).GetComponent<CommandObject>() == null)
                                    continue;

                                Command command = new Command
                                {
                                    CommandText = commandPrefab.GetComponent<CommandObject>().commandText
                                        .Replace("<Name>", controller.gameObject.name).Replace("<Target>",
                                            this.transform.parent.GetChild(j).name),
                                    CommandObject = commandPrefab,
                                    PlayerTransform = obj.transform,
                                    TargetTransform = this.transform.parent.GetChild(j)
                                };

                                if (this.commands.All(c => c.CommandText != command.CommandText))
                                    this.commands.Add(command);
                            }
                        }
                    }
                }

                foreach (GameObject commandPrefab in this.commandPrefabs)
                {
                    if (commandPrefab.GetComponent<CommandObject>().commandText.Contains("<Name>"))
                        continue;

                    Command command = new Command
                    {
                        CommandText = commandPrefab.GetComponent<CommandObject>().commandText
                    };

                    if (this.commands.All(c => c.CommandText != command.CommandText))
                        this.commands.Add(command);
                }
            }
        }

        public List<string> GetStringCommands() =>
            this.commands.Select(c => c.CommandText).ToList();

        public void SpawnCommand(int commandIndex)
        {
            GameObject obj = Instantiate(this.commands[commandIndex].CommandObject, Vector3.zero, Quaternion.identity,
                this.commands[commandIndex].PlayerTransform);

            if (obj.GetComponent<TargetCommandObject>() is { } commandObject)
                commandObject.SetTarget(this.commands[commandIndex].TargetTransform);
        }
    }

    internal struct Command
    {
        public string CommandText;
        public GameObject CommandObject;
        public Transform PlayerTransform;
        public Transform TargetTransform;
    }
}