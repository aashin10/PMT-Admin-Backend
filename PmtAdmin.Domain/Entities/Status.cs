using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Entities
{
    [Table("status")]
    public class Status
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("status_name")]
        public string? StatusName { get; set; }

        // Navigation properties
        public ICollection<BoardColumn> BoardColumns { get; set; }
    }

}











//using PmtAdmin.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PmtAdmin.Domain.Entities
//{
//    [Table("status")]
//    public class Status
//    {
//        [Key]
//        [Column("id")]
//        public Guid Id { get; set; }

//        [Column("status_name")]
//        public string? StatusName { get; set; }

//        // Navigation properties
//        public ICollection<BoardColumn> BoardColumns { get; set; }
//    }

//}




//userside change

//[Key]
//[Column("id")]
//[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//public int Id { get; set; }

//[Required]
//[Column("status_name")]
//public string StatusName { get; set; }

//// Navigation properties
//public ICollection<BoardColumn> BoardColumns { get; set; }
