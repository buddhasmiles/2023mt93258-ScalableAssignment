using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    /// <summary>
    /// Contains common properties used by other tables
    /// </summary>

    public class BaseModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public bool IsActive { get; set; } = true;

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public DateTime? DateUpdated { get; set; } = null;
    }
}
