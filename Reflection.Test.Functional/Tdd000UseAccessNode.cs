using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Reflection.Test.Functional.Models;

namespace Reflection.Test.Functional
{
    public class Tdd000UseAccessNode : TestBase
    {
        public override void Main()
        {
            var node = ObjectStructure.CreateStructure(typeof(Person), true);

            new AccessNodePrinter().Print(node);

            PrintLine();

            var person = new Person
            {
                Id = 1,
                Name = "FirstPerson",
                Job = new Job
                {
                    Id = 11,
                    Title = "First-Job",
                    Description = "First Job Of All Time"
                },
                Addresses = new List<Address>
                {
                    new Address
                    {
                        Id = 111,
                        Name = "FirstAddress",
                        WrittenAddress = "1---1-1--1-1-1--1"
                    },
                    new Address
                    {
                        Id = 112,
                        Name = "SecondAddress",
                        WrittenAddress = "-3--2-2-2-2--3-3-2--2"
                    }
                }
            };

            PrintObject(person);

            var leaves = node.EnumerateLeavesBelow();
            var leavesById = new Dictionary<string,AccessNode>();
            
            
            foreach (var leaf in leaves)
            {
                Console.WriteLine("Leaf: " + leaf.GetFullName()
                                           + $" : {leaf.Type.Name} = {ReadLeaf(leaf, person)}");
                
                leavesById.Add(leaf.GetFullName(),leaf);
            }
            
            var kherson = new Person();
            
            WriteLeaf(leavesById["Person.Id"],kherson,12);
            WriteLeaf(leavesById["Person.Name"],kherson,"Mr Bear!");
            WriteLeaf(leavesById["Person.Addresses.Address[-1]"],kherson,person.Addresses[0]);
            WriteLeaf(leavesById["Person.Job.Title"],kherson,"NotNullJob!");
            
            PrintLine();
            
            PrintObject(kherson);
            
            
            PrintLine();
            
            
            var personClone  = new Person();
            
            foreach (var leaf in leaves)
            {
                var value = ReadLeaf(leaf, person);
                
                WriteLeaf(leaf,personClone,value);
            }
            
            PrintObject(personClone);
            
            
            PrintTitle("Standard Object");
            
            
            var sEvaluator = new ObjectEvaluator(person);
            
            var dEvaluator = new ObjectEvaluator(person.GetType());

            var standard = sEvaluator.ToStandardFlatData();
            
            standard.ForEach( d => Console.WriteLine($"{d.Identifier}: {d.Value}"));
            
            standard.ForEach(d => dEvaluator.Write(d.Identifier,d.Value));
            
            PrintLine();
            
            PrintTitle("Check for indexing access");

            var addresses = sEvaluator.Read("Person.Addresses");
            
            var lastAddress = sEvaluator.Read("Person.Addresses.Address[-1]");
            
            var indexedAddress = sEvaluator.Read("Person.Addresses.Address[0]");
            
            Console.WriteLine("All Addresses.");
            
            PrintObject(addresses);

            Console.WriteLine("LastAddress.");
            
            PrintObject(lastAddress);
            
            PrintLine();
            
            Console.WriteLine("Indexed Address.");
            
            PrintObject(indexedAddress);
        }

        private object ReadLeaf(AccessNode leaf, object rootObject)
        {
            if (leaf.IsRoot)
            {
                return rootObject;
            }

            var parentObject = ReadLeaf(leaf.Parent, rootObject);

            return leaf.Evaluator.Read(parentObject);
        }

        private void WriteLeaf(AccessNode leaf, object rootObject, object value)
        {
            if (leaf.IsRoot)
            {
                Console.WriteLine("What the hell??");
                return;
            }

            var parentNode = leaf.Parent;

            var parentObject = ReadLeaf(parentNode, rootObject);

            if (parentObject == null)
            {
                parentObject = new TypeAnalyzer().CreateObject(parentNode.Type,true);
                
                WriteLeaf(parentNode,rootObject,parentObject);
            }
            leaf.Evaluator.Write(parentObject, value);
            
            PrintLine();
        }
    }
}