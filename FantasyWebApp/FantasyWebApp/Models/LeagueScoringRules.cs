using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FantasyWebApp.Models
{
    public class LeagueScoringRules
    {
        public int Id { get; set; }
        public int DraftId { get; set; }

        //set to ESPN league standard for default 
        //QB points
        public float PassingYardsPointsPerYard { get; set; } = 0.04f;     //0.4 per pass yd
        public float PassingTouchdownPoints { get; set; } = 4f;           //4.0 per TD
        public float InterceptionPoints { get; set; } = -2f;              //-2.0 per INT

        //can add options for 300yd, 400yd, 500yd performances

        //scrimmage points
        public float RushingYardsPointsPerYard { get; set; } = 0.1f;      //0.1 per rush yd 
        public float RushingTouchdownPoints { get; set; } = 6f;           //6.0 per rush TD
        public float ReceivingYardsPointsPerYard { get; set; } = 0.1f;    //0.1 per rec yd 
        public float ReceptionPoints { get; set; } = 1f;                    //1.0 PPR will be used as standard
        public float ReceivingTouchdownPoints { get; set; } = 6f;           //6.0 per rec TD
        public float FumbleLostPoints { get; set; } = -2f;                  //-2.0 per fumble

        //can add options for 100yd, 150yd, 200yd performances

        public float FieldGoalMadePoints { get; set; } = 3f;                //3.0 per standard field goal
        public float FieldGoalMade40Points { get; set; } = 4f;              //4.0 per 40+ yd field goal
        public float FieldGoalMade50Points { get; set; } = 5f;              //5.0 per 50+ yd field goal
        public float FieldGoalMade60Points { get; set; } = 6f;              //6.0 per 60+ yd field goal
        public float ExtraPointMadePoints { get; set; } = 1f;               //1.0 per XP
        public float DefenseSackPoints { get; set; } = 1f;                  //1.0 per def sack
        public float DefenseInterceptionPoints { get; set; } = 2f;          //2.0 per def int
        public float DefenseFumbleRecoveryPoints { get; set; } = 2f;        //2.0 per def fumble recovery
        public float DefenseTouchdownPoints { get; set; } = 6f;             //6.0 per D/ST TD
        public float DefenseSafetyPoints { get; set; } = 2f;                //2.0 per safety
        public float DefensePointsAllowed0Points { get; set; } = 10f;       //points for not allowing points
        public float DefensePointsAllowed1To6Points { get; set; } = 7f;
        public float DefensePointsAllowed7To13Points { get; set; } = 4f;
        public float DefensePointsAllowed14To20Points { get; set; } = 1f;
        public float DefensePointsAllowed21To27Points { get; set; } = 0f;
        public float DefensePointsAllowed28To34Points { get; set; } = -1f;  //often disabled
        public float DefensePointsAllowed35PlusPoints { get; set; } = -4f;  //often disabled
    }
}