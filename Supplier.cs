using Alachisoft.NCache.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo
{
    [QueryIndexable]
    [Serializable]
    public class Supplier
    {
        /// <summary>
        /// The unique identifier for the supplier.
        /// </summary>
        public int SupplierId { get; set; }

        /// <summary>
        /// The legal name of the supplier company.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// The primary contact person at the supplier.
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// The supplier's primary phone number.
        /// </summary>
        public string Phone { get; set; }

        public DateTime LastModify { get; set; }

        public override string ToString()
        {
            return $"ID: {SupplierId}, Company: {CompanyName}, Contact: {ContactName}";
        }
    }

}
