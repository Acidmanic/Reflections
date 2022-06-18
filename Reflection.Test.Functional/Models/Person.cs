using System.Collections.Generic;

namespace Reflection.Test.Functional.Models
{
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Address> Addresses { get; set; }

        public Job Job { get; set; }
    }
}