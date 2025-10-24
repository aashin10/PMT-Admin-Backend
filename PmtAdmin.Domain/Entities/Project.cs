using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PmtAdmin.Domain.Entities
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? Key { get; set; }

        public string? Description { get; set; }

        // Customer Information
        [MaxLength(255)]
        public string? CustomerOrgName { get; set; }

        [MaxLength(255)]
        public string? CustomerDomainUrl { get; set; }

        public string? CustomerDescription { get; set; }

        [MaxLength(255)]
        public string? PocEmail { get; set; }

        [MaxLength(50)]
        public string? PocPhone { get; set; }

        // Management & Status
        public int? ProjectManagerId { get; set; }
        public int? ProjectManagerRoleId { get; set; }
        public int? StatusId { get; set; }
        public int? DeliveryUnitId { get; set; }
        public int? CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsImportedFromJira { get; set; }
        public int? TemplateId { get; set; }

        // Navigation Properties
        [ForeignKey("ProjectManagerId")]
        public virtual User? ProjectManager { get; set; }

        [ForeignKey("ProjectManagerRoleId")]
        public virtual Role? ProjectManagerRole { get; set; }

        //[ForeignKey("StatusId")]
        //public virtual ProjectStatus Status { get; set; }

        //[ForeignKey("DeliveryUnitId")]
        //public virtual DeliveryUnit DeliveryUnit { get; set; }

        [ForeignKey("CreatedById")]
        public virtual User? CreatedBy { get; set; }

        [ForeignKey("UpdatedById")]
        public virtual User? UpdatedBy { get; set; }

        [ForeignKey("TemplateId")]
        public virtual ProjectTemplate? Template { get; set; }
        public virtual ICollection<Team>? Teams { get; set; }
        public virtual ICollection<Board>? Boards { get; set; }
        public virtual ICollection<ProjectMember>? Members { get; set; }

    }
}
