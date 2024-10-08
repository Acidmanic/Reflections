using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Xsl;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Reflection.Test.Functional;

public class Tdd25Parameters : TestBase
{
    private record IdRecord([AutoValuedMember] int Property);

    private record UniqueRecord([UniqueMember] int Property);

    private record DefaultValueRecord([DefaultValue(10)] int Property);

    private record TreatAsLeafRecord([TreatAsLeaf] object Property);

    private record MemberNameRecord([MemberName("Lie")] int Property);

    private record NoneDataFieldRecord([IgnoreNoneDataNode] int Property);

    private class NoneDataFieldClass
    {
        [IgnoreNoneDataNode] public int Property { get; set; }
    };

    private interface INoneDataFieldClassBase
    {
        [IgnoreNoneDataNode] public int Property { get; set; }
    };
    
    private interface INoneDataFieldRecordBase
    {
        [IgnoreNoneDataNode] public int Property { get; init; }
    };
    
    private class NoneDataFieldClassDriven:INoneDataFieldClassBase
    {
        public int Property { get; set; }

    };

    private record NoneDataFieldRecordDriven(int Property) : INoneDataFieldRecordBase;

    public override void Main()
    {
        IdRecord_Should_Has_One_Id_Child();

        UniqueRecord_Should_Has_One_Unique_Child();

        DefaultValueRecord_Should_Has_One_DefaultValued_Child();

        TreatAsLeafRecord_Should_Has_One_TreatAsLeaf_Child();

        MemberNameRecord_Should_Has_One_Child_Named_Lie();

        NoneDataFieldType_Should_Not_Have_Property<NoneDataFieldRecord>();

        NoneDataFieldType_Should_Not_Have_Property<NoneDataFieldClass>();
        
        NoneDataFieldType_Should_Not_Have_Property<NoneDataFieldClassDriven>();
        
        NoneDataFieldType_Should_Not_Have_Property<NoneDataFieldRecordDriven>();
    }


    private void IdRecord_Should_Has_One_Id_Child()
    {
        var ev = new ObjectEvaluator(typeof(IdRecord));

        var leaf = ev.RootNode.GetChildren().First();

        if (!leaf.IsAutoValued) throw new Exception("Expected Id Field.");
    }

    private void UniqueRecord_Should_Has_One_Unique_Child()
    {
        var ev = new ObjectEvaluator(typeof(UniqueRecord));

        var leaf = ev.RootNode.GetChildren().First();

        if (!leaf.IsUnique) throw new Exception("Expected Unique Field.");
    }

    private void DefaultValueRecord_Should_Has_One_DefaultValued_Child()
    {
        var ev = new ObjectEvaluator(typeof(DefaultValueRecord));

        var leaf = ev.RootNode.GetChildren().First();

        if (leaf.PropertyAttributes.FirstOrDefault(a => a is DefaultValueAttribute) is DefaultValueAttribute attribute)
        {
            if ((int)attribute.Value! != 10)
            {
                throw new Exception("Default Value's value is not correct");
            }
        }
        else
        {
            throw new Exception("Expected Unique Field.");
        }
    }

    private void TreatAsLeafRecord_Should_Has_One_TreatAsLeaf_Child()
    {
        var ev = new ObjectEvaluator(typeof(TreatAsLeafRecord));

        var leaf = ev.RootNode.GetChildren().First();

        if (!leaf.IsLeaf) throw new Exception("Expected Leaf Here Field.");
    }

    private void MemberNameRecord_Should_Has_One_Child_Named_Lie()
    {
        var ev = new ObjectEvaluator(typeof(MemberNameRecord));

        var leaf = ev.RootNode.GetChildren().First();

        if (leaf.Name != "Lie") throw new Exception("Expected a Field here named Lie.");
    }


    private void NoneDataFieldType_Should_Not_Have_Property<T>()
    {
        var ev = new ObjectEvaluator(typeof(T));

        if (ev.RootNode.GetChildren().Count > 0)
        {
            throw new Exception(
                $"{typeof(T).Name} Should not have any leaves " +
                $"but it had {ev.RootNode.GetChildren().Count}");
        }
    }
}