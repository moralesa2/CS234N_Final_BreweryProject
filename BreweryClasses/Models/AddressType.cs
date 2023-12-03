using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BreweryClasses.Models
{
    public partial class AddressType
    {
        public AddressType()
        {
            SupplierAddresses = new HashSet<SupplierAddress>();
        }

        public int AddressTypeId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<SupplierAddress> SupplierAddresses { get; set; }

        public override string ToString()
        {
            return AddressTypeId + ", " + Name;
        }
    }
}
