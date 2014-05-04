using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace Models
{
    public class ClanInvitationsViewModel
    {
        public List<ClanInvitation> ClanInvitations { get; set; }
        public Clan Clan { get; set; }
        public bool isLeader { get; set; }
    }
}