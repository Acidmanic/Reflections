using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Xsl;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Reflection.Test.Functional;

public static class AccessNodeExtensions
{
    public static string XmlName(this AccessNode node)
    {
        if (node.IsCollectable)
        {
            return FieldKey.Parse(node.GetFullName()).TerminalSegment().Name;
        }

        return node.Name;
    }
}

public class Tdd24ScanNodes : TestBase
{
    private record A(int Id, string Name);

    private record B(int Id, string Title);

    private record C(int Id, B B);

    private class Model
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public List<string> Links { get; set; }

        public List<A> AValues { get; set; }

        public C CValue { get; set; }
    }


    public override void Main()
    {
        var model = new Model()
        {
            Links = new List<string>
            {
                "https://www.google.com",
                "https://linkedin.com"
            },
            Name = "InstanceModel",
            DisplayName = "Instance Model",
            AValues = new List<A>
            {
                new A(1, "A-1"),
                new A(2, "A-2"),
            },
            CValue = new C(3, new B(4, "b4-Value"))
        };

        var evaluator = new ObjectEvaluator(model);

        var sb = new StringBuilder();

        evaluator.ScanNodes(((node, key, value, terminal) =>
        {
            sb.Append('<').Append(node.XmlName())
                .Append(' ')
                .Append("address='").Append(key.ToString()).Append("'")
                .Append('>');
            if (terminal)
            {
                sb.Append(value);
            }
        }), (node, key, value, terminal) => { sb.Append('<').Append('/').Append(node.XmlName()).Append('>'); });


        var xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + sb.ToString();

        Console.WriteLine(xml);

        var output = Path.Join(new FileInfo(Assembly.GetEntryAssembly()?.Location!).Directory?.FullName, "output.xml");

        if (File.Exists(output)) File.Delete(output);

        File.WriteAllText(output, xml);
    }
}