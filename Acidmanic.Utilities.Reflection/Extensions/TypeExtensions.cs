using System;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Acidmanic.Utilities.Reflection.Extensions
{
    public static class TypeExtensions
    {
        public static AccessNode GetUniqueIdFieldLeaf(this Type type)
        {
            var node = new TypeAnalyzer().ToAccessNode(type, false);

            var leaves = node.GetDirectLeaves();

            var idLeaf = leaves.FirstOrDefault(l => l.IsUnique);

            return idLeaf;
        }

        public static string GetIdFieldName(this Type type)
        {

            var idLeaf = type.GetUniqueIdFieldLeaf();

            return idLeaf?.Name;
        }
    }
}