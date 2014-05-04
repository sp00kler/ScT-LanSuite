using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Comments : IModel
    {
        [Required]
        public string ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "UserNameRequired")]
        [Display(Name="UserName", ResourceType = typeof(Resources.Lang))]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "ContentRequired")]
        [Display(Name = "Content", ResourceType=typeof(Resources.Lang))]
        [MaxLength(4000)]
        public string Content { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "DateRequired")]
        [Display(Name = "Date", ResourceType=typeof(Resources.Lang))]
        public DateTime Date { get; set; }

        public string NewsID { get; set; }

        [ForeignKey("NewsID")]
        public virtual News News { get; set; }

        public Comments() 
        {
            this.ID = Guid.NewGuid().ToString();
        }
    }
}
