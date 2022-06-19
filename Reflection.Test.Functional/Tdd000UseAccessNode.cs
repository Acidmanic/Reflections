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
                        WrittenAddress = "1---1-1--1-1-1--1",
                        Pins = new List<Pin>
                        {
                            new Pin {Id = 1, Name = "F1-Pin"}
                        }
                    },
                    new Address
                    {
                        Id = 112,
                        Name = "SecondAddress",
                        WrittenAddress = "-3--2-2-2-2--3-3-2--2",
                        Pins = new List<Pin>
                        {
                            new Pin {Id = 2, Name = "S1-Pin"},
                            new Pin {Id = 3, Name = "S1-Pin22"}
                        }
                    }
                }
            };

            PrintObject(person);

            var leaves = node.EnumerateLeavesBelow();
            var leavesById = new Dictionary<string, AccessNode>();

            PrintTitle("Standard Object");

            var sEvaluator = new ObjectEvaluator(person);
            
            PrintLine();

            PrintTitle("Check for indexed access");

            //var addresses = sEvaluator.Read("Person.Addresses");

            var indexedPin = sEvaluator.Read("Person.Addresses.Address[1].Pins.Pin[0]");

            PrintTitle("Before Writing:");

            PrintObject(indexedPin);

            sEvaluator.Write("Person.Addresses.Address[1].Pins.Pin[0].Name","Maskharah!");
            
            PrintTitle("After Writing:");

            PrintObject(indexedPin);
            
            
            PrintTitle("Verify Standard leaves");
            
            var standard = sEvaluator.ToStandardFlatData();

            standard.ForEach(d => Console.WriteLine($"{d.Identifier}: {d.Value}"));
            
            
            PrintTitle("Verify Cloning");
            
            var dEvaluator = new ObjectEvaluator(person.GetType());

            standard.ForEach(d => dEvaluator.Write(d.Identifier,d.Value));

            PrintObject(dEvaluator.RootObject);
        }
    }
}