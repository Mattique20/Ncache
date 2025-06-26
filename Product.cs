using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Serialization;
using Alachisoft.NCache.Runtime.Serialization.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo
{
  
    [QueryIndexable]
    public class Product : ICompactSerializable
    {
        
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public double Price { get; set; }
        
        public string Category { get; set; }
        public DateTime LastModify { get; set; }

        public Product() { }

        public Product(int attrib1, string attrib2, double attrib3)
        {
            Id = attrib1;
            Name = attrib2;
            Price = attrib3;
        }
        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Price: {Price:C}";
        }

        #region ICompactSerializable Members

        public void Deserialize(CompactReader reader)
        {
            Id = reader.ReadInt32();
            Name = reader.ReadObject() as string;
            Price = reader.ReadDouble();
        }

        public void Serialize(CompactWriter writer)
        {
            writer.Write(Id);
            writer.WriteObject(Name);
            writer.Write(Price);
        }

        #endregion
    }


}
