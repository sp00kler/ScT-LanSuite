using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class News : IModel
    {
        [Required]
        public string ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "TitleRequired")]
        [Display(Name = "Title", ResourceType = typeof(Resources.Lang))]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "ContentRequired")]
        [Display(Name = "Content", ResourceType = typeof(Resources.Lang))]
        [MaxLength(4000)]
        public string Content { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "DateRequired")]
        [Display(Name = "Date", ResourceType = typeof(Resources.Lang))]
        public DateTime Date { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "PlaceRequired")]
        [Display(Name = "Place", ResourceType = typeof(Resources.Lang))]
        public int Place { get; set; }
        
        [Required]
        public string PageID { get; set; }

        [Required]
        [ForeignKey("PageID")]
        public virtual Page Page { get; set; }

        public virtual IList<Comments> Comments { get; set; }

        public News() 
        {
            this.ID = Guid.NewGuid().ToString();
        }
    }
}