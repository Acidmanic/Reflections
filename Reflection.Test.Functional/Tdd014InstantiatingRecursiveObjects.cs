using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Litbid.DataAccess.Models;

namespace Reflection.Test.Functional
{
    /// <summary>
    /// This test shows:
    ///     for fully support recursive references in object tree, both ObjectInstantiator.CreateObject(),
    ///     and ObjectStructure.CreateStructure() must support avoiding infinite recursions. Sirect recursions
    ///     is easy to trace since a direct recursion happens when the type of parent node and the child is the same.
    ///     but that is not always the case, and for fully supported version, both methods need to keep a list of the
    ///     parent nodes being passed down the current child, and use this list to make sure there is no infinite
    ///     recursion going on by searching the list for current child type. this can also decrease the performance
    ///     slightly.
    ///     For now  ObjectInstantiator.CreateObject(), only supports (avoid infinite recursion) for direct cases.
    ///     Both ObjectInstantiator and ObjectStructure currently regard the TreatAsLeaf attribute and will not dig
    ///     further when they hit one.
    /// </summary>
    public class Tdd014InstantiatingRecursiveObjects : TestBase
    {

        class BadObject
        {
            public long Id { get; set; }
            
            public BadObject BadProperty { get; set; }
        }
        
        class NotThatBadObject
        {
            public long Id { get; set; }
            
            [TreatAsLeaf]
            public BadObject BadProperty { get; set; }
        }
        
        public override void Main()
        {

            var instance = new ObjectInstantiator().CreateObject<BadObject>(true);
            
            var otherInstance = new ObjectInstantiator().CreateObject<NotThatBadObject>(true);

            var accessNode = ObjectStructure.CreateStructure<NotThatBadObject>(true);
            
             var otherEvaluator = new ObjectEvaluator(typeof(NotThatBadObject));
        }
    }
}