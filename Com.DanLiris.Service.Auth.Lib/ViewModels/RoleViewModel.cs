using Com.DanLiris.Service.Auth.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.DanLiris.Service.Auth.Lib.ViewModels
{
    public class RoleViewModel : BasicOldViewModel, IValidatableObject
    {
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public ICollection<PermissionViewModel> permissions { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            int Count = 0;

            if (string.IsNullOrWhiteSpace(this.code))
                yield return new ValidationResult("Code is required", new List<string> { "code" });

            if (string.IsNullOrWhiteSpace(this.name))
                yield return new ValidationResult("Name is required", new List<string> { "name" });

            string permissionError = "[";

            foreach (PermissionViewModel permission in permissions)
            {
                if (string.IsNullOrWhiteSpace(permission.unit.name))
                {
                    Count++;
                    permissionError += "{ unit: 'Unit is required' }, ";
                }
                else
                {
                    permissionError += "{}, ";
                }
            }

            permissionError += "]";

            if (Count > 0)
            {
                yield return new ValidationResult(permissionError, new List<string> { "unit" });
            }
        }
    }
}
