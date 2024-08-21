// using System;
// using System.Collections.Generic;
// using System.Runtime.InteropServices;
// using Acidmanic.Utilities.Reflection;
// using Acidmanic.Utilities.Reflection.Attributes;
// using Acidmanic.Utilities.Reflection.Extensions;
// using Acidmanic.Utilities.Reflection.FieldInclusion;
// using Acidmanic.Utilities.Reflection.ObjectTree;
// using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
// using Litbid.DataAccess.Models;
//
// namespace Reflection.Test.Functional
// {
//     public class Tdd012AlteredCasts : TestBase
//     {
//         private class Inner
//         {
//             public long Id { get; set; }
//
//             public string Name { get; set; }
//         }
//
//         [AlteredType(typeof(string))]
//         private class CsvAddress
//         {
//             public string Country { get; set; }
//
//             public string State { get; set; }
//
//             public string City { get; set; }
//             
//             public static implicit operator string(CsvAddress address)
//             {
//                 if (address == null)
//                 {
//                     return null;
//                 }
//
//                 return address.Country + "," + address.State + "," + address.City;
//             }
//
//             public static implicit operator CsvAddress(string address)
//             {
//                 if (address == null)
//                 {
//                     return null;
//                 }
//
//                 var segments = address.Split(",");
//
//                 return new CsvAddress
//                 {
//                     Country = segments.Length > 0 ? segments[0] : null,
//                     State = segments.Length > 1 ? segments[1] : null,
//                     City = segments.Length > 2 ? segments[2] : null
//                 };
//             }
//         }
//
//         private class Outer
//         {
//             public long Id { get; set; }
//
//             public string Name { get; set; }
//
//             public virtual CsvAddress Address { get; set; }
//
//             public Inner InnerData { get; set; }
//         }
//         
//         private class AlternateOuter:Outer
//         {
//             [TreatAsLeaf]
//              public override CsvAddress Address { get; set; }
//
//
//             public AlternateOuter()
//             {
//                 
//             }
//
//             public AlternateOuter(Outer outer)
//             {
//                 Address = outer.Address;
//                 Id = outer.Id;
//                 Name = outer.Name;
//                 InnerData = outer.InnerData;
//                 
//             }
//         }
//
//         public override void Main()
//         {
//             var fullData = new Outer
//             {
//                 Address = new CsvAddress
//                 {
//                     Country = "Iran",
//                     State = "Tehran",
//                     City = "Tehran"
//                 },
//                 Id = 12,
//                 Name = "Example",
//                 InnerData = new Inner
//                 {
//                     Id = 23,
//                     Name = "Test"
//                 }
//             };
//
//             var alteredData = new AlternateOuter(fullData);
//
//             var evaluator = new ObjectEvaluator(fullData);
//             
//             var regularFlat = evaluator.ToStandardFlatData();
//
//             evaluator = new ObjectEvaluator(alteredData);
//             
//             var castedFlat = evaluator.ToStandardFlatData(o => o.UseAlternativeTypes());
//             
//             PrintRecord(regularFlat);
//             
//             PrintLine();
//             
//             PrintRecord(castedFlat);
//
//
//         }
//     }
// }