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
        }
    }
}