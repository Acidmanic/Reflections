using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Extensions;
using Reflection.Test.Functional.Models;

namespace Reflection.Test.Functional
{
    public class Tdd003TestCloneAbility : TestBase
    {
        public override void Main()
        {
            var person = new Person
            {
                Addresses = new List<Address>
                {
                    new Address
                    {
                        Id = 10,
                        Name = "Home",
                        Pins = new List<Pin>
                        {
                            new Pin
                            {
                                Id = 11,
                                Name = "Pin1"
                            }
                        },
                        WrittenAddress = "Tehran"
                    }
                },
                Id = 12,
                Job = new Job
                {
                    Description = "Cool job",
                    Id = 13,
                    Title = "The Cool Specialist"
                },
                Name = "Mani Moayedi"
            };


            var myClone = person.Clone();

            PrintObject(myClone);


            var equality = person.AreEquivalentsWith(myClone);

            Console.WriteLine("Clone and original are " + (equality ? "" : " NOT ") + "Equal");
        }
    }
}