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

    [Table("board_columns")]
    public class BoardColumn
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }


        [Column("status_id")]
        public int? StatusId { get; set; }

        [Column("board_column_name")]
        public string? BoardColumnName { get; set; }

        [Column("board_color")]
        public string? BoardColor { get; set; }

        [Column("position")]
        public int? Position { get; set; }

        // Navigation properties
        [ForeignKey("StatusId")]
        public Status? Status { get; set; }
        public ICollection<BoardBoardColumnMap>? BoardBoardColumnMaps { get; set; }

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

//    [Table("board_columns")]
//    public class BoardColumn
//    {
//        [Key]
//        [Column("id")]
//        public Guid Id { get; set; }

//        [Column("status_id")]
//        public Guid? StatusId { get; set; }

//        [Column("board_name")]
//        public string? BoardName { get; set; }

//        [Column("board_color")]
//        public string? BoardColor { get; set; }

//        [Column("position")]
//        public int? Position { get; set; }

//        // Navigation properties
//        [ForeignKey("StatusId")]
//        public Status? Status { get; set; }
//    }

//}


