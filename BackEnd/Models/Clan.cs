using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Clan : IModel
    {
        [Required]
        public string ID { get; set; }

        [Required(ErrorMessageResourceType= typeof(Resources.Lang), ErrorMessageResourceName="TitleRequired")]
        [Display(Name = "Title", ResourceType=typeof(Resources.Lang))]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "TagRequired")]
        [Display(Name = "Tag", ResourceType = typeof(Resources.Lang))]
        [MaxLength(10)]
        public string Tag { get; set; }

        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUser Leader { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        public Clan() 
        {
            this.ID = Guid.NewGuid().ToString();
        }

    }
}