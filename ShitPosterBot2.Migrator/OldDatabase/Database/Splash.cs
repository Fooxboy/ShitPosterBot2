﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShitPosterBot.Database
{
    public class Splash
    {
        [Key]
        public long Id { get; set; }

        public string? Message { get; set; }

    }
}
