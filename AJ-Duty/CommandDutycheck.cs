using Rocket.Unturned.Player;
using Rocket.API;
using Rocket.Unturned.Chat;
using System.Collections.Generic;

namespace AJUN
{
    public class CommandDutycheck : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, Duty.Instance.Translate("error_dc_usage"));
            }
            else if (command.Length > 0)
            {
                UnturnedPlayer cplayer = UnturnedPlayer.FromName(command[0]);
                Duty.Instance.CDuty(cplayer, caller);
            }
        }

        public string Help
        {
            get { return "Checks if a player has admin powers or not."; }
        }

        public string Name
        {
            get { return "DutyCheck"; }
        }

        public string Syntax
        {
            get { return "<playername>"; }
        }

        public bool AllowFromConsole
        {
            get { return true; }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Both; }
        }
        public List<string> Aliases
        {
            get { return new List<string>() { "dc" }; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "duty.check" };
            }
        }
    }
}
