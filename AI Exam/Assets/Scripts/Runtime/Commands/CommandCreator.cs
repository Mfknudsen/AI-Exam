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
        private static List<Command> commands;

        [SerializeField] private  List<GameObject> commandPrefabs;

        private void Start()
        {
            commands = new List<Command>();

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

                            if (commands.All(c => c.CommandText != command.CommandText))
                                commands.Add(command);
                        }
                        else
                        {
                            for (int j = 0; j < this.transform.parent.childCount; j++)
                            {
                                if (i == j)
                                    continue;

                                if (this.transform.parent.GetChild(j).GetComponent<PlayerController>() == null)
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
                                
                                if (commands.All(c => c.CommandText != command.CommandText))
                                    commands.Add(command);
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
                        CommandText = commandPrefab.GetComponent<CommandObject>().commandText,
                        CommandObject = commandPrefab,
                        PlayerTransform = transform
                    };

                    if (commands.All(c => c.CommandText != command.CommandText))
                        commands.Add(command);
                }
            }
        }

        public static List<string> GetStringCommands() =>
            commands.Select(c => c.CommandText).ToList();

        public static void SpawnCommand(int commandIndex)
        {
            GameObject obj = Instantiate(commands[commandIndex].CommandObject, Vector3.zero, Quaternion.identity,
                commands[commandIndex].PlayerTransform);

            if (obj.GetComponent<TargetCommandObject>() is { } commandObject)
                commandObject.SetTarget(commands[commandIndex].TargetTransform);
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