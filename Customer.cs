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
    public class Customer
    {
        /// <summary>
        /// The unique identifier for the customer.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// The first name of the customer.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the customer.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The shipping address for the customer.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The city of the customer.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The country of the customer.
        /// </summary>
        public string Country { get; set; }

        public override string ToString()
        {
            return $"ID: {CustomerId}, Name: {FirstName} {LastName}, Location: {City}, {Country}";
        }
    }
}
