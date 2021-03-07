using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using Rocket.Unturned;
using Rocket.Core.Plugins;
using Rocket.API.Collections;
using Rocket.API;
using Rocket.Core;
using System.Collections.Generic;
using Rocket.API.Serialisation;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;
using System.Linq;
using System;
using SDG.Unturned;
using Steamworks;
using System.Collections.Specialized;

namespace AJUN
{
    public class Duty : RocketPlugin<DutyConfiguration>
    {
        public static Duty Instance;
        public List<DutyGroups> ValidGroups;
        public static string Webhook = "";

        protected override void Load()
        {
            Instance = this;

            ValidGroups = Configuration.Instance.Groups;

            foreach (DutyGroups Group in ValidGroups.ToList())
            {
                RocketPermissionsGroup g = R.Permissions.GetGroup(Group.GroupID);
                if (g == null)
                {
                    Logger.LogWarning("Permission group " + Group.GroupID + " does not exist! No command related to that group will execute.");
                    ValidGroups.Remove(Group);
                }
            }


            Logger.LogWarning("Loading event \"Player Connected\"...");
            U.Events.OnPlayerConnected += PlayerConnected;
            Logger.LogWarning("Loading event \"Player Disconnected\"...");
            U.Events.OnPlayerDisconnected += PlayerDisconnected;
            Logger.LogWarning("Loading deathevents.");
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerDeath += onplayerdeath;

            Logger.LogWarning("");
            Logger.LogWarning("Duty has been successfully loaded!");
        }

        protected override void Unload()
        {
            Instance = null;

            Logger.LogWarning("Unloading on player connect event...");
            U.Events.OnPlayerConnected -= PlayerConnected;
            Logger.LogWarning("Unloading on player disconnect event...");
            U.Events.OnPlayerConnected -= PlayerDisconnected;
            Logger.LogWarning("Unloading deathevents.");
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerDeath -= onplayerdeath;

            Logger.LogWarning("");
            Logger.LogWarning("Duty has been unloaded!");
        }

        private void onplayerdeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murder)
        {
            var murder3 = UnturnedPlayer.FromCSteamID(murder);

            if (cause == EDeathCause.SENTRY || cause == EDeathCause.LANDMINE || cause == EDeathCause.VEHICLE || cause == EDeathCause.FOOD || cause == EDeathCause.WATER || cause == EDeathCause.ZOMBIE || cause == EDeathCause.ANIMAL || cause == EDeathCause.INFECTION || cause == EDeathCause.BOULDER) { }
            try
            {
                var red = Color.red;
                if (murder3.GodMode == true)
                {
                    UnturnedChat.Say(red + player.CharacterName + " died by a staff-member in godmode. ABUSER: " + murder3.CharacterName);
                    HTTP.Post(Webhook, new NameValueCollection()
            {
                { "username", "Duty/IsAbusing Link" },
                { "avatar_url", "https://cdn.discordapp.com/attachments/696080024742395914/718483498947838063/beetlejuice-1.jpg" },
                { "content", "" + player.CharacterName + " has died to " + murder3.CharacterName + " (" + murder3.CSteamID + ")" + " while they were in godmode." }
            });
                }
                else if (murder3.VanishMode == true)
                {   
                    UnturnedChat.Say(red + player.CharacterName + " died to a staff-member in vanish. ABUSER: " + murder3.CharacterName);

                    HTTP.Post(Webhook, new NameValueCollection()
            {
                { "username", "Duty/IsAbusing Link" },
                { "avatar_url", "https://cdn.discordapp.com/attachments/696080024742395914/718483498947838063/beetlejuice-1.jpg" },
                { "content", "" + player.CharacterName + " has died to " + murder3.CharacterName + " (" + murder3.CSteamID + ")" + " while they were in vanish." }
            });
                }
                else if (murder3.IsAdmin == true)
                {
                    HTTP.Post(Webhook, new NameValueCollection()
            {
                { "username", "Duty/IsAbusing Link" },
                { "avatar_url", "https://cdn.discordapp.com/attachments/696080024742395914/718483498947838063/beetlejuice-1.jpg" },
                { "content", "" + player.CharacterName + " has died to " + murder3.CharacterName + " (" +murder3.CSteamID + ")" + " while they were on duty." }
            });
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning("DUTY ERROR | " + e);
            }
        }

        public void DoDuty(UnturnedPlayer caller)
        {
            if (caller.IsAdmin)
            {
                caller.Admin(false);
                caller.Features.GodMode = false;
                caller.Features.VanishMode = false;
                caller.Player.look.sendFreecamAllowed(false);
                caller.Player.look.sendSpecStatsAllowed(false);
                caller.Player.look.sendWorkzoneAllowed(false);
                /* SDG.Unturned.EffectManager.askEffectClearByID(9000, caller.CSteamID); */
                if (Configuration.Instance.EnableServerAnnouncer) UnturnedChat.Say(Instance.Translate("off_duty_message", caller.CharacterName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
            }
            else
            {
                /* SDG.Unturned.EffectManager.sendUIEffect(9000, 9000, caller.CSteamID, false, Color.red + "You are on duty.");*/
                caller.Admin(true);
                caller.Player.look.sendFreecamAllowed(true);
                caller.Player.look.sendSpecStatsAllowed(true);
                caller.Player.look.sendWorkzoneAllowed(true);
                if (Configuration.Instance.EnableServerAnnouncer) UnturnedChat.Say(Instance.Translate("on_duty_message", caller.CharacterName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
            }
        }

        public void DoDuty(UnturnedPlayer Player, DutyGroups Group)
        {
            RocketPermissionsGroup Target = R.Permissions.GetGroup(Group.GroupID);
            if (Target.Members.Contains(Player.CSteamID.ToString()))
            {
                R.Permissions.RemovePlayerFromGroup(Target.Id, Player);
                Player.Features.GodMode = false;
                Player.Features.VanishMode = false;
                if (Configuration.Instance.EnableServerAnnouncer) UnturnedChat.Say(Instance.Translate("off_duty_message", Player.DisplayName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
                return;
            }
            else
            {
                R.Permissions.AddPlayerToGroup(Group.GroupID, Player);
                if (Configuration.Instance.EnableServerAnnouncer) UnturnedChat.Say(Instance.Translate("on_duty_message", Player.DisplayName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
            }
        }

        public void CDuty(UnturnedPlayer cplayer, IRocketPlayer caller)
        {
            if (!Configuration.Instance.AllowDutyCheck)
            {
                UnturnedChat.Say(caller, Translate("error_unable_checkduty"));
                return;
            }
            if (cplayer != null && cplayer.IsAdmin)
            {
                if (caller is ConsolePlayer)
                {
                    UnturnedChat.Say(Instance.Translate("check_on_duty_message", "Console", cplayer.DisplayName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
                }
                else if (caller is UnturnedPlayer)
                {
                    UnturnedChat.Say(Instance.Translate("check_on_duty_message", caller.DisplayName, cplayer.DisplayName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
                }
                return;
            }
            else if (cplayer != null)
            {
                foreach (DutyGroups Group in ValidGroups)
                {
                    RocketPermissionsGroup Target = R.Permissions.GetGroup(Group.GroupID);
                    if (Target.Members.Contains(cplayer.CSteamID.ToString()))
                    {
                        if (caller is ConsolePlayer)
                        {
                            UnturnedChat.Say(Instance.Translate("check_on_duty_message", "Console", cplayer.DisplayName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
                        }
                        else if (caller is UnturnedPlayer)
                        {
                            UnturnedChat.Say(Instance.Translate("check_on_duty_message", caller.DisplayName, cplayer.DisplayName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
                        }
                        return;
                    }
                }
            }
            else if (cplayer == null)
            {
                UnturnedChat.Say(caller, Translate("error_cplayer_null"));
            }
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList {
                    {"admin_login_message", "{0} has logged on and is now on duty."},
                    {"admin_logoff_message", "{0} has logged off and is now off duty."},
                    {"on_duty_message", "{0} is now on duty."},
                    {"off_duty_message", "{0} is now off duty."},
                    {"check_on_duty_message", "{0} has confirmed that {1} is on duty."},
                    {"check_off_duty_message", "{0} has confirmed that {1} is not on duty."},
                    {"not_enough_permissions", "You do not have the correct permissions to use this command."},
                    {"error_unable_checkduty", "Unable To Check Duty. Configuration Is Set To Be Disabled."},
                    {"error_cplayer_null", "Player is not online or his name is invalid." },
                    {"error_dc_usage", "No argument was specified. Please use \"dc <playername>\" to check on a player." }
                };
            }
        }

        void PlayerConnected(UnturnedPlayer player)
        {
            if (player.IsAdmin)
            {
                if (Configuration.Instance.EnableServerAnnouncer) UnturnedChat.Say(Instance.Translate("admin_login_message", player.CharacterName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
                return;
            }

            foreach (DutyGroups Group in ValidGroups)
            {
                RocketPermissionsGroup Target = R.Permissions.GetGroup(Group.GroupID);
                if (Target.Members.Contains(player.CSteamID.ToString()))
                {
                    if (Configuration.Instance.EnableServerAnnouncer) UnturnedChat.Say(Instance.Translate("admin_login_message", player.CharacterName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
                    return;
                }
            }
        }

        void PlayerDisconnected(UnturnedPlayer player)
        {
            if (player.IsAdmin)
            {
                if (Configuration.Instance.RemoveDutyOnLogout)
                {
                    player.Admin(false);
                    player.Features.GodMode = false;
                    player.Features.VanishMode = false;
                }

                if (Configuration.Instance.EnableServerAnnouncer) UnturnedChat.Say(Instance.Translate("admin_logoff_message", player.CharacterName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
                return;
            }

            foreach (DutyGroups Group in ValidGroups)
            {
                RocketPermissionsGroup Target = R.Permissions.GetGroup(Group.GroupID);
                if (Target.Members.Contains(player.CSteamID.ToString()))
                {
                    if (Configuration.Instance.RemoveDutyOnLogout)
                    {
                        player.Features.GodMode = false;
                        player.Features.VanishMode = false;
                        R.Permissions.RemovePlayerFromGroup(Target.Id, player);
                    }

                    if (Configuration.Instance.EnableServerAnnouncer) UnturnedChat.Say(Instance.Translate("admin_logoff_message", player.CharacterName), UnturnedChat.GetColorFromName(Instance.Configuration.Instance.MessageColor, Color.red));
                    return;
                }
            }
        }
    }
}
