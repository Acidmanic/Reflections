using System;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Reflection.Test.Functional;

public class Tdd23RecordsPropertyAttributes : TestBase
{


    private class Ta : Attribute
    {
    }

    private record Rec([Ta]string Value);

    public override void Main()
    {

        var evaluator = new ObjectEvaluator(typeof(Rec));

        var node = evaluator.RootNode.GetChildren().First();

        var attributes = node.PropertyAttributes;
        
        
    }
}