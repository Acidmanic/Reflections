using System;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Reflection.Test.Functional;

public class Tdd21StructAccessNodes : TestBase
{
    private struct Id
    {
        public long Value { get; set; }
    }

    private struct Child
    {
        public Id Id { get; set; }

        public string Name { get; set; }
    }

    private class Model
    {
        public Id Id { get; set; }

        public Child Child { get; set; }
    }

    public override void Main()
    {
        var model = new Model
        {
            Child = new Child
            {
                Id = new Id { Value = 10 },
                Name = "Mashti"
            },
            Id = new Id() { Value = 20 }
        };

        var evaluator = new ObjectEvaluator(model);

        var rootNode = evaluator.RootNode;

        var leaves = rootNode.GetDirectLeaves().Select(l => evaluator.Map.AddressByNode(l));

        foreach (var leaf in leaves)
        {
            Console.WriteLine(evaluator.Read(leaf));
        }
    }
}