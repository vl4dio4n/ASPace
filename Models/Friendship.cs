using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ASPace.Areas.Identity.Data;

namespace ASPace.Models{
    public class Friendship{
        public string? FirstId { get; set; }
        public string? SecondId { get; set; }
        public DateTime AcceptDate { get; set; }

        // [ForeignKey("FirstId")]
        public virtual ApplicationUser? First { get; set; }
        // [ForeignKey("SecondId")]
        public virtual ApplicationUser? Second { get; set; }
        
    }
}