using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Team : IModel
    {
        [Required]
        public string ID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [Display(Name = "Title Name")]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Tag is required.")]
        [Display(Name = "Tag")]
        [MaxLength(10)]
        public string Tag { get; set; }

        public string CompoID { get; set; }
        public string UserID { get; set; }

        [ForeignKey("CompoID")]
        public virtual Compo Compo { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUser Captain { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }


        public Team()
        {
            this.ID = Guid.NewGuid().ToString();
        }
    }
}