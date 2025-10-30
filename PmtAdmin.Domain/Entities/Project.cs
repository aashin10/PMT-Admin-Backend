using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PmtAdmin.Domain.Entities
{

    [Table("projects")]
    public class Project
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("name")]
        public string Name { get; set; }

        [MaxLength(50)]
        [Column("key")]
        public string? Key { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        // Customer Information
        [MaxLength(255)]
        [Column("customer_org_name")]
        public string? CustomerOrgName { get; set; }

        [MaxLength(255)]
        [Column("customer_domain_url")]
        public string? CustomerDomainUrl { get; set; }

        [Column("customer_description")]
        public string? CustomerDescription { get; set; }

        [MaxLength(255)]
        [Column("poc_email")]
        public string? PocEmail { get; set; }

        [MaxLength(50)]
        [Column("poc_phone")]
        public string? PocPhone { get; set; }

        // Management & Status
        [Column("project_manager_id")]
        public int? ProjectManagerId { get; set; }

        [Column("project_manager_role_id")]
        public int? ProjectManagerRoleId { get; set; }

        [Column("status_id")]
        public int? StatusId { get; set; }

        [Column("delivery_unit_id")]
        public int? DeliveryUnitId { get; set; }

        [Column("created_by")]
        public int? CreatedBy { get; set; }

        [Column("updated_by")]
        public int? UpdatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("metadata", TypeName = "jsonb")]
        public string? Metadata { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("isimportedfromjira")]
        public bool? IsImportedFromJira { get; set; }

        [Column("template_id")]
        public int? TemplateId { get; set; }

        // Navigation properties
        [ForeignKey("ProjectManagerId")]
        public User? ProjectManager { get; set; }

        [ForeignKey("ProjectManagerRoleId")]
        public Role? ProjectManagerRole { get; set; }

        [ForeignKey("StatusId")]
        public ProjectStatus? Status { get; set; }

        [ForeignKey("DeliveryUnitId")]
        public DeliveryUnit? DeliveryUnit { get; set; }

        [ForeignKey("CreatedBy")]
        public User? Creator { get; set; }

        [ForeignKey("UpdatedBy")]
        public User? Updater { get; set; }

        [ForeignKey("TemplateId")]
        public ProjectTemplate? Template { get; set; }

        public ICollection<Team> Teams { get; set; }
        public ICollection<Board> Boards { get; set; }
        public ICollection<ProjectMember> ProjectMembers { get; set; }
        public ICollection<Sprint> Sprints { get; set; }
        public ICollection<Epic> Epics { get; set; }
        public ICollection<Issue> Issues { get; set; }
        public ICollection<CustomField> CustomFields { get; set; }

    }
}
