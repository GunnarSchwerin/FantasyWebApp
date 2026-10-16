using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FantasyWebApp.Models
{
    public class Draft
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = "Fantasy Draft";

        public int TeamCount { get; set; } = 12;
        public int RoundCount { get; set; } = 15;
        public bool IsSnake { get; set; } = true;
        public string Status { get; set; } = "Setup"; // Setup, Live, Complete
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        public virtual ICollection<DraftTeam> Teams { get; set; }
        public virtual ICollection<DraftPick> Picks { get; set; }
    }
}
