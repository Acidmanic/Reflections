using System.Collections.Generic;

namespace Reflection.Test.Functional.Models
{
    public class Address
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string WrittenAddress { get; set; }

        public List<Pin> Pins { get; set; }
    }
}