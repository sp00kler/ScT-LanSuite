﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models
{
    public class Seating : IModel
    {
        [Required]
        public string ID { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        [Required]
        public virtual Edition Edition { get; set; }
    }
}
