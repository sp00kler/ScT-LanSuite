using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ClanSeating : IModel
    {
        [Required]
        public string ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "TableNameRequired")]
        [MaxLength(100)]
        [Display(Name = "TableName", ResourceType = typeof(Resources.Lang))]
        public string TableName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "SeatNameRequired")]
        [MaxLength(100)]
        [Display(Name = "SeatName", ResourceType = typeof(Resources.Lang))]
        public string SeatName { get; set; }

        public string ClanID { get; set; }

        public string EditionID { get; set; }
        
        [ForeignKey("ClanID")]
        public virtual Clan Clan { get; set; }

        [ForeignKey("EditionID")]
        public virtual Edition Edition { get; set; }

        
    }
}
