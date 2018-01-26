using Com.DanLiris.Service.Auth.Lib.Services;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.DanLiris.Service.Auth.Lib.Models
{
    public class Role : StandardEntity, IValidatableObject
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
        public virtual ICollection<AccountRole> AccountRoles { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            /* Service Validation */
            RoleService service = (RoleService)validationContext.GetService(typeof(RoleService));

            if (service.DbContext.Set<Role>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Code.Equals(this.Code)) > 0) /* Unique */
            {
                yield return new ValidationResult("Code already exists", new List<string> { "code" });
            }
        }
    }
}
