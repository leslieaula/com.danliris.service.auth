using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.DanLiris.Service.Auth.Lib.Models
{
    public class AccountRole : StandardEntity, IValidatableObject
    {
        public int AccountId { get; set; }
        public virtual Account Account { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
