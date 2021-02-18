using Rocket.API;
using System.Collections.Generic;

namespace AJUN
{
    public class DutyConfiguration : IRocketPluginConfiguration
    {
        public bool EnableServerAnnouncer;
        public bool RemoveDutyOnLogout;
        public bool AllowDutyCheck;
        public string MessageColor;
        public string SuperAdminPermission;
        public List<DutyGroups> Groups;


        public void LoadDefaults()
        {
            EnableServerAnnouncer = true;
            RemoveDutyOnLogout = true;
            AllowDutyCheck = true;
            MessageColor = "red";
            SuperAdminPermission = "duty.superadmin";
            Groups = new List<DutyGroups>()
            {
                new DutyGroups("Administrator", "duty.admin"),
                new DutyGroups("Moderator", "duty.mod"),
                new DutyGroups("Helper", "duty.helper"),
            };
        }
    }

    public class DutyGroups
    {
        public string GroupID;
        public string Permission;

        public DutyGroups() { GroupID = "Administrator"; Permission = "admin"; }
        public DutyGroups(string ID, string Perm)
        {
            GroupID = ID;
            Permission = Perm;
        }
    }
}
