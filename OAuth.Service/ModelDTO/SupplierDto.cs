﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth.Service.ModelDto
{
   public  class SupplierDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SupplierName { get; set; }
        public int Status { get; set; }
        public string HandleName { get; set; }
        public DateTime? HandleTime { get; set; }
        public string MobilePhone { get; set; }
        public string FixedPhone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string Fax { get; set; }
        public string BankName { get; set; }
        public string BankNo { get; set; }
        public string TaxNo { get; set; }
        public int? PayTime { get; set; }
        public string Memo { get; set; }
    }
}
