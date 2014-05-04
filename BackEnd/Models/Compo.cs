using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Compo : IModel
    {
        [Required]
        public string ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "TitleRequired")]
        [Display(Name = "Title", ResourceType = typeof(Resources.Lang))]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "RulesRequired")]
        [Display(Name = "Rules", ResourceType = typeof(Resources.Lang))]
        [MaxLength(500)]
        public string Rules { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Lang), ErrorMessageResourceName = "GameRequired")]
        [Display(Name = "Game", ResourceType = typeof(Resources.Lang))]
        [MaxLength(100)]
        public string Game { get; set; }

        public virtual ICollection<Team> Teams { get; set; }

        public Compo() 
        {
            this.ID = Guid.NewGuid().ToString();
        }


    }
}
