using Alachisoft.NCache.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo.SampleData
{
    [Serializable]
    public class OrderDetail
    {
        public int ProductId { get; set; }

       
        public double UnitPrice { get; set; }

        public int Quantity { get; set; }
    }

   
    [QueryIndexable]
    [Serializable]
    public class Order
    {
        
        public int OrderId { get; set; }
        public int CustomerId { get; set; }

        
        public DateTime OrderDate { get; set; }

       
        public DateTime RequiredDate { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }

        public override string ToString()
        {
            return $"ID: {OrderId}, CustomerID: {CustomerId}, Date: {OrderDate.ToShortDateString()}, Items: {OrderDetails.Count}";
        }
    }
}
