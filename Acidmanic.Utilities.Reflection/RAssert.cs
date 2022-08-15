using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Exceptions;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Record = Acidmanic.Utilities.Reflection.ObjectTree.StandardData.Record;


namespace Acidmanic.Utilities.Reflection
{
    public class RAssert
    {

        private static Action<object, object> AssertEqual = (ex, ac) =>
        {
            if (!ex.AreEqualAsNullables(ac))
            {
                throw new EqualException($"\nExpected:{ex}\nActual: {ac}");
            }
        };
        
        
        private static Action<bool> AssertTrue = (value) =>
        {
            if (!value)
            {
                throw new EqualException($"\nExpected value to be 'True', but it was 'False'.");
            }
        };
        
        private static Action<DataPoint, DataPoint> AssertEqualDataPoints = (ex, ac) =>
        {
            AssertEqual(ex.Identifier, ac.Identifier);
            
            if (!ex.Value.AreEqualAsNullables(ac.Value))
            {
                throw new EqualException("\nExpected: " + ex.Identifier + ": " + ex.Value ,
                    "\nActual: " + ac.Identifier + ": " + ac.Value);
            }
        };
        
        
        public static void Equal(object expected, object actual)
        {
            SameNullState(expected, actual);

            if (expected != null && actual != null)
            {
                var expEvaluator = new ObjectEvaluator(expected);

                var actEvaluator = new ObjectEvaluator(actual);

                var expStandard = expEvaluator.ToStandardFlatData();

                var actStandard = actEvaluator.ToStandardFlatData();

                Equal(expStandard, actStandard);
            }
        }

        private class ByIdComparator : IComparer<DataPoint>
        {
            public int Compare(DataPoint x, DataPoint y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                if (x == null)
                {
                    return -1;
                }

                if (y == null)
                {
                    return 1;
                }

                return string.Compare(x.Identifier, y.Identifier);
            }
        }

        public static void Equal(Record expected, Record actual)
        {
            SameNullState(expected, actual);

            SameCount(expected, actual);

            var comp = new ByIdComparator();

            actual.Sort(comp);
            expected.Sort(comp);

            for (int i = 0; i < expected.Count; i++)
            {
                var ex = expected[i];

                var ac = actual[i];

                AssertEqualDataPoints(ex, ac);
            }
        }

        
        
        
        public static void SameCount<T>(ICollection<T> expected, ICollection<T> actual)
        {
            AssertEqual(expected.Count, actual.Count);
        }

        public static void SameNullState<T>(T expected, T actual)
        {
            AssertTrue((expected == null && actual == null) || (expected != null && actual != null));
        }
    }
}