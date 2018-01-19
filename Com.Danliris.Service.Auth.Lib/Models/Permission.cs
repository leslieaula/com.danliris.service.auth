using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Auth.Lib.Models
{
    public class Permission : StandardEntity, IValidatableObject
    {
        public int UnitId { get; set; }
        public string UnitCode { get; set; }
        public string Unit { get; set; }
        public string Division { get; set; }
        public int permission { get; set; }

        public int RoleId { get; set; }
        public virtual Role Role { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
