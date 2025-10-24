using System.ComponentModel.DataAnnotations;

namespace PmtAdmin.Domain.Entities
{
    public class ProjectTemplate
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }
}
