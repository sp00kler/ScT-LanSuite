using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;


namespace Models
{
    public class Page : IModel
    {
        [Required]
        public string ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "TitleRequired")]
        [Display(Name = "Title", ResourceType = typeof(Resources.Lang))]
        [MaxLength(100)]
        public string Title { get; set; }

        //[Required(ErrorMessage = "Content is required.")]
        [Display(Name = "Content", ResourceType = typeof(Resources.Lang))]
        [MaxLength(4000)]
        [AllowHtml]
        public string Content { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "PlaceRequired")]
        [Display(Name = "Place", ResourceType = typeof(Resources.Lang))]
        public int Place { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "IsNewsRequired")]
        [Display(Name = "IsNews", ResourceType = typeof(Resources.Lang))]
        public bool isNews { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "IsActivatedRequired")]
        [Display(Name = "IsActivated", ResourceType = typeof(Resources.Lang))]
        public bool isActivated { get; set; }

        public virtual IList<News> News { get; set; }

        [NotMapped]
        public bool isNew { get; set; }
        
        public Page() 
        {
            this.ID = Guid.NewGuid().ToString();
            //this.isNew = true;
        }
    }
}