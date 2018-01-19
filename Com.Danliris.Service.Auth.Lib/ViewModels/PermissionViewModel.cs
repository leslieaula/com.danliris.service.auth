using System;
using System.Collections.Generic;
using System.Text;

namespace Com.DanLiris.Service.Auth.Lib.ViewModels
{
    public class PermissionViewModel
    {
        public int id { get; set; }
        public DateTime _createdDate { get; set; }
        public string _createdBy { get; set; }
        public string _createAgent { get; set; }
        public UnitViewModel unit { get; set; }
        public int permission { get; set; }
        public int roleId { get; set; }
    }
}
