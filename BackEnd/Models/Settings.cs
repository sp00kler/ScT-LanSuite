﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Settings
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }

    }
}
