using Alachisoft.NCache.Runtime.Serialization;
using Alachisoft.NCache.Runtime.Serialization.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo
{
    class DummyCompactSerialization : ICompactSerializable
    {
        private string _attribute1;
        private int _attribute2;
        private bool _attribute3;

        public DummyCompactSerialization()
        { }

        public DummyCompactSerialization(string attrib1, int attrib2, bool attrib3)
        {
            _attribute1 = attrib1;
            _attribute2 = attrib2;
            _attribute3 = attrib3;
        }

        #region ICompactSerializable Members

        public void Deserialize(CompactReader reader)
        {
            _attribute1 = reader.ReadObject() as string;
            _attribute2 = reader.ReadInt32();
            _attribute3 = reader.ReadBoolean();
        }

        public void Serialize(CompactWriter writer)
        {
            writer.WriteObject(_attribute1);
            writer.Write(_attribute2);
            writer.Write(_attribute3);
        }

        #endregion
    }
}
