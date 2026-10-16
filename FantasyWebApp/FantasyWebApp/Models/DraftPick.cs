using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FantasyWebApp.Models
{
    public class DraftPick
    {
        public int Id { get; set; }     //set id for db
        public int DraftId { get; set; }    //id of the draft
        public int Round { get; set; }      //round info
        public int PickInRound { get; set; }    //draft pick info
        public int OverallPick { get; set; }    //calculated using round and pick

        // CurrentTeamId determines who makes the pick. OriginalTeamId preserves history after trades.
        public int OriginalTeamId { get; set; }
        public int CurrentTeamId { get; set; }

        public bool IsKeeperPick { get; set; }  
        [StringLength(120)]
        public string KeeperPlayerName { get; set; }

        public bool IsTraded { get; set; }  //for traded draft picks
        [StringLength(500)]
        public string TradeNote { get; set; }   //optional trade notes??

        public int? DraftPlayerId { get; set; }
        public bool IsCompleted { get; set; }   //is draft over

        public virtual Draft Draft { get; set; }
        public virtual DraftTeam OriginalTeam { get; set; }
        public virtual DraftTeam CurrentTeam { get; set; }
        public virtual DraftPlayer DraftPlayer { get; set; }
    }
}