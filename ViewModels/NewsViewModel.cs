using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class NewsViewModel
    {
        public News News { get; set; }
        public string DateShown { get; set; }
        public List<NewsCommentViewModel> Comments { get; set; }
    }
}
