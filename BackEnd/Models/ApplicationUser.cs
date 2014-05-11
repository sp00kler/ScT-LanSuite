using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string ConfirmationToken { get; set; }
        public string ClanID { get; set; }
        [ForeignKey("ClanID")]
        public virtual Clan Clan { get; set; }
        public virtual List<ClanInvitation> ClanInvitations { get; set; }
    }
}
