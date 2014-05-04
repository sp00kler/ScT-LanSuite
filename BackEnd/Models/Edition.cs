using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Edition : IModel
    {
        [Required]
        public string ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "TitleRequired")]
        [Display(Name = "Title", ResourceType = typeof(Resources.Lang))]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "SeatsRequired")]
        [Display(Name = "Seats", ResourceType = typeof(Resources.Lang))]
        public int Seats { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "PlaceRequired")]
        [Display(Name = "Place", ResourceType = typeof(Resources.Lang))]
        public int Place { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "IsActivatedRequired")]
        [Display(Name = "IsActivated", ResourceType = typeof(Resources.Lang))]
        public bool isActivated {get;set;}
   
        public virtual Seating Seating { get; set; }

        [NotMapped]
        public bool isNew { get; set; }

        public Edition() 
        {
            this.ID = Guid.NewGuid().ToString();
        }
    }
}
