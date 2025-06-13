using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo
{
    [Serializable]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

        public string Category { get; set; }
        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Price: {Price:C}";
        }
    }


}
