namespace PmtAdmin.Domain.Entities
{
    public class Sprint
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? SprintGoal { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }

    }
}
