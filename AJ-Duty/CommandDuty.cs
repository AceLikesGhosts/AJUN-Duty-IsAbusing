using Rocket.Unturned.Player;
using Rocket.API;
using System.Collections.Generic;

namespace AJUN
{
    public class CommandDuty : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (caller == null) return;
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (caller.HasPermission(Duty.Instance.Configuration.Instance.SuperAdminPermission))
            {
                Duty.Instance.DoDuty(player);
                return;
            }

            foreach (DutyGroups Group in Duty.Instance.ValidGroups)
            {
                if (caller.HasPermission(Group.Permission))
                {
                    Duty.Instance.DoDuty(player, Group);
                    return;
                }
            }

            if (caller.HasPermission("duty")) Rocket.Unturned.Chat.UnturnedChat.Say(caller, Duty.Instance.Translate("not_enough_permissions"));
        }

        public string Help
        {
            get { return "Gives admin powers to the player without the need of the console."; }
        }

        public string Name
        {
            get { return "duty"; }
        }

        public string Syntax
        {
            get { return ""; }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { "d" }; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "duty" };
            }
        }
    }
}
