using Com.DanLiris.Service.Auth.Lib.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.DanLiris.Service.Auth.Lib.ViewModels
{
    public class AccountViewModel : BasicOldViewModel, IValidatableObject
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool isLocked { get; set; }
        public AccountProfileViewModel profile { get; set; }
        public List<RoleViewModel> roles { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            int Count = 0;

            if (string.IsNullOrWhiteSpace(this.username))
                yield return new ValidationResult("Username is required", new List<string> { "username" });

            if (this._id.Equals(0) && string.IsNullOrWhiteSpace(this.password))
                yield return new ValidationResult("Password is required", new List<string> { "password" });

            if (this.profile.Equals(null) || string.IsNullOrWhiteSpace(this.profile.firstname))
                yield return new ValidationResult("{ firstname: 'First Name is required' }", new List<string> { "profile" });

            string accountRoleError = "[";

            foreach (RoleViewModel role in roles)
            {
                if (role._id.Equals(0))
                {
                    Count++;
                    accountRoleError += "{ name: 'Role is required' }, ";
                }
                else
                {
                    accountRoleError += "{}, ";
                }
            }

            accountRoleError += "]";

            if (Count > 0)
            {
                yield return new ValidationResult(accountRoleError, new List<string> { "roles" });
            }
        }
    }
}
