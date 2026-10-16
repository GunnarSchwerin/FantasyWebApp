using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FantasyWebApp.Models
{
    public class DraftPlayer
    {
        public int Id { get; set; }         //give id

        [Required, StringLength(120)]
        public string Name { get; set; }    //player name

        [StringLength(10)]
        public string Position { get; set; }    //position of player

        [StringLength(10)]
        public string NflTeam { get; set; }     //team they play for IRL

        public int? ByeWeek { get; set; }       //bye week of NFL team
        public decimal? ProjectedPassingYards { get; set; }     //this information will be scraped from table
        public decimal? ProjectedPassingTouchdowns { get; set; }
        public decimal? ProjectedInterceptions { get; set; }
        public decimal? ProjectedRushingYards { get; set; }
        public decimal? ProjectedRushingTouchdowns { get; set; }
        public decimal? ProjectedReceivingYards { get; set; }
        public decimal? ProjectedReceptions { get; set; }
        public decimal? ProjectedReceivingTouchdowns { get; set; }
        public decimal? ProjectedFumblesLost { get; set; }
        public decimal? ProjectedFieldGoalsMade { get; set; }
        public decimal? ProjectedExtraPointsMade { get; set; }
        public decimal? ProjectedDefensePointsAllowed { get; set; }
        public decimal? ProjectedDefenseSacks { get; set; }
        public decimal? ProjectedDefenseInterceptions { get; set; }
        public decimal? ProjectedDefenseFumbleRecoveries { get; set; }
        public decimal? ProjectedDefenseTouchdowns { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}