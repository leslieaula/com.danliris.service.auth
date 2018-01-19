using Com.DanLiris.Service.Auth.Lib.Services;
using Com.Moonlay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.DanLiris.Service.Auth.Lib.Models
{
    public class Account : StandardEntity, IValidatableObject
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsLocked { get; set; }
        public virtual AccountProfile AccountProfile { get; set; }
        public virtual ICollection<AccountRole> AccountRoles { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            /* Service Validation */
            AccountService service = (AccountService)validationContext.GetService(typeof(AccountService));

            if (service.DbContext.Set<Account>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Username.Equals(this.Username)) > 0) /* Unique */
            {
                yield return new ValidationResult("Username already exists", new List<string> { "username" });
            }
        }
    }
}
