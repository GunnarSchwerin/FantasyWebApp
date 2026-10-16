using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FantasyWebApp.Models
{
    public class DraftTeam
    {
        public int Id { get; set; }         //id of league??
        public int DraftId { get; set; }    //id of this draft

        [Required, StringLength(80)]
        public string Name { get; set; }    //Name of the team

        // Team's original position in the first-round order, 1-based.
        public int DraftPosition { get; set; }  //position in the draft

        public virtual Draft Draft { get; set; }    //get draft info
        public virtual ICollection<DraftPick> Picks { get; set; }   //all of your draft picks
    }
}