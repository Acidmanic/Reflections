using System;

namespace Acidmanic.Utilities.Reflection.ObjectTree
{
    public class AccessNodePrinter
    {
        public void Print(AccessNode node)
        {
            Print("", node);
        }

        private void Print(string indent, AccessNode node, bool fullTypeName = false)
        {
            Console.Write(indent + "Name: " + node.Name);
            Console.Write(", Of Type: " + (fullTypeName ? node.Type.FullName : node.Type.Name));
            Console.Write(", Collectable: " + node.IsCollection);
            Console.Write(", Leaf: " + node.IsLeaf);
            Console.Write(", Root: " + node.IsRoot);
            Console.Write(", Unique: " + node.IsUnique);
            Console.Write(", Depth: " + node.Depth);
            Console.WriteLine();
            var children = node.GetChildren();
            foreach (var child in children)
            {
                Print(indent + "    ", child);
            }
        }
    }
}