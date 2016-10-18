using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Data.Enum;

namespace Zhengdi.Framework.Data.Entity

{
    public interface IField
    {
        string Name { get; }
        Type Type { get; }
        dynamic Value { get;  }
         
    }
    public interface IProperty: IField
    {
 
        bool Ignore { get; }
        bool SetAliaName(IRelevance relevanc);
        PrimaryType PrimaryType { get; set; }
        bool PrimaryKey { get; }
        bool SetTableName(string name);
        void SetPrimaryKey(PrimaryType Type);
        IProperty IsJoin(bool status);
        bool Equals(IProperty property);
 
    }

    public interface IRelevance
    {
        IProperty PrimaryKey { get; }
        string AliaName { get; }
        IProperty ForeignKey { get; }
        Type Type { get; }
        IProperty EntityProperty { get; }
    }
}
