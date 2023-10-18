using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShitPosterBot.Database
{
    public class Statistics
    {
        [Key]
        public long Id { get; set; }

        public long? Ads { get; set; }

        public long? Updates { get; set; }

        public long? Messages { get; set; }

    }
}
