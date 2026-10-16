namespace FantasyWebApp.Models
{
    // Values are decimal so fractional scoring such as 0.1 per scrimmage yard is supported.
    public class LeagueScoringRule
    {
        public int Id { get; set; }
        public int DraftId { get; set; }

        public decimal PassingYardsPointsPerYard { get; set; } = 0.04m;
        public decimal PassingTouchdownPoints { get; set; } = 4m;
        public decimal InterceptionPoints { get; set; } = -2m;

        public decimal RushingYardsPointsPerYard { get; set; } = 0.1m;
        public decimal RushingTouchdownPoints { get; set; } = 6m;
        public decimal ReceivingYardsPointsPerYard { get; set; } = 0.1m;
        public decimal ReceptionPoints { get; set; } = 1m;
        public decimal ReceivingTouchdownPoints { get; set; } = 6m;
        public decimal FumbleLostPoints { get; set; } = -2m;

        public decimal FieldGoalMadePoints { get; set; } = 3m;
        public decimal ExtraPointMadePoints { get; set; } = 1m;
        public decimal DefenseSackPoints { get; set; } = 1m;
        public decimal DefenseInterceptionPoints { get; set; } = 2m;
        public decimal DefenseFumbleRecoveryPoints { get; set; } = 2m;
        public decimal DefenseTouchdownPoints { get; set; } = 6m;
        public decimal DefenseSafetyPoints { get; set; } = 2m;
        public decimal DefensePointsAllowed0Points { get; set; } = 10m;
        public decimal DefensePointsAllowed1To6Points { get; set; } = 7m;
        public decimal DefensePointsAllowed7To13Points { get; set; } = 4m;
        public decimal DefensePointsAllowed14To20Points { get; set; } = 1m;
        public decimal DefensePointsAllowed21To27Points { get; set; } = 0m;
        public decimal DefensePointsAllowed28To34Points { get; set; } = -1m;
        public decimal DefensePointsAllowed35PlusPoints { get; set; } = -4m;
    }
}
