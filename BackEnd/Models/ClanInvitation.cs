using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ClanInvitation : IModel
    {
        [Required]
        public string ID { get; set; }
        public string ClanID { get; set; }
        public string UserID { get; set; }
        [ForeignKey("ClanID")]
        public virtual Clan Clan { get; set; }
        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        public ClanInvitation() 
        {
            this.ID = Guid.NewGuid().ToString();
        }
    }
}
