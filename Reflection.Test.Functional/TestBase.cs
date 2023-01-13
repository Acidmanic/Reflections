using System;
using System.Collections;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;

namespace Reflection.Test.Functional
{
    public abstract class TestBase
    {
        public abstract void Main();

        protected void PrintObject(object o)
        {
            PrintObject("", o);
        }

        private void PrintObject(string indent, object obj)
        {
            if (obj == null)
            {
                Console.WriteLine("[NULL]");
                return;
            }

            if (TypeCheck.IsCollection(obj.GetType()) && obj is IEnumerable objects)
            {
                Console.WriteLine(indent + "[");

                foreach (var o in objects)
                {
                    PrintNonEnumerableObject(indent, o);
                }

                Console.WriteLine(indent + "]");
            }
            else
            {
                PrintNonEnumerableObject(indent, obj);
            }
        }

        protected void PrintLine()
        {
            Console.WriteLine("----------------------------------------------------------------");
        }

        protected void PrintTitle(string title)
        {
            PrintLine();

            Console.WriteLine("\t\t\t" + title);

            PrintLine();
        }

        private void PrintNonEnumerableObject(string indent, object o)
        {
            Line(indent, 30);


            if (o == null)
            {
                Console.WriteLine(indent + "NULL");

                return;
            }

            var type = o.GetType();


            if (!TypeCheck.IsReferenceType(type))
            {
                Console.WriteLine(indent + o);

                return;
            }

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (property.CanRead)
                {
                    var name = property.Name;
                    var value = property.GetValue(o);

                    if (value == null || IsSingleLinable(value.GetType()))
                    {
                        Console.WriteLine(indent + name + ": " + value);
                    }
                    else
                    {
                        Console.WriteLine(indent + property.Name);

                        PrintObject(indent + "    ", value);
                    }
                }
            }
        }

        private void Line(string caption, int lineLength)
        {
            var length = lineLength - caption.Length;

            string line = caption;

            for (int i = 0; i < length; i++)
            {
                line += "-";
            }

            Console.WriteLine(line);
        }

        private bool IsSingleLinable(Type type)
        {
            if (type.IsPrimitive || type.IsValueType)
            {
                return true;
            }

            if (type == typeof(string) || type == typeof(char))
            {
                return true;
            }

            return false;
        }

        protected void PrintRecord(Record record)
        {
            foreach (var dataPoint in record)
            {
                Console.WriteLine(dataPoint.Identifier+": " + dataPoint.Value.ToString());
            }
        }
    }
}