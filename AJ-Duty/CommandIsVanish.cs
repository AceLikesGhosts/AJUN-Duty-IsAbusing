using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace AJUN
{
    class CommandIsVanish : IRocketCommand
    {
        public static UnturnedPlayer player2;

        public string Help
        {
            get { return "See's if the user is in vanish."; }
        }

        public string Name
        {
            get { return "isvanish"; }
        }

        public string Syntax
        {
            get { return "[Player]"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Both; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "duty.isvanish" };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 1)
            {
                player2 = UnturnedPlayer.FromName(command[0]);
                if (player2.VanishMode == true)
                {
                    UnturnedChat.Say(player2.CharacterName + " Has vanish enabled");
                }
                else
                {
                    UnturnedChat.Say(player2.CharacterName + " Has vanish disabled");
                }
            }

        }
    }
}