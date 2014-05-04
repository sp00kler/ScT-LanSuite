using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Registration : IModel
    {
        [Required]
        public string ID { get; set; }

        [Required(ErrorMessage = "Paid is required.")]
        [Display(Name = "Paid")]
        public bool Paid {get;set;}

        public string UserID { get; set; }
        public string EditionID { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("EditionID")]
        public virtual Edition Edition { get; set; }

        public Registration()
        {
            this.ID = Guid.NewGuid().ToString();
        }
    }
}
