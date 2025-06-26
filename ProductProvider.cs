using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Serialization;
using Alachisoft.NCache.Runtime.Serialization.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Providers
{
    [Serializable]
    [QueryIndexable]
    public class ProductProvider : ICompactSerializable
    {
            
            public int Id { get; set; }

            
            public string Name { get; set; }

            
            public decimal Price { get; set; }

            
            public int CategoryId { get; set; }

            

            public ProductProvider() { }

            public override string ToString()
            {
               
                return $"ID: {Id}, Name: {Name}, Price: {Price:C}, CategoryID: {CategoryId}";
            }

            #region ICompactSerializable Members

            
            public void Deserialize(CompactReader reader)
            {
                Id = reader.ReadInt32();
                Name = reader.ReadObject() as string;
                Price = reader.ReadDecimal();
                CategoryId = reader.ReadInt32();
            }

            
            public void Serialize(CompactWriter writer)
            {
                writer.Write(Id);
                writer.WriteObject(Name);
                writer.Write(Price);
                writer.Write(CategoryId);
            }

            #endregion
    }
}


