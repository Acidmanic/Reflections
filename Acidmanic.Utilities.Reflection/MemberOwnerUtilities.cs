using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Acidmanic.Utilities.Reflection
{
    /// <summary>
    /// This class provides functionalities to deal with a class as a data and it's fields as the members of this data
    /// and their standard or costume (provided using attributes) names 
    /// </summary>
    public static class MemberOwnerUtilities
    {


        public static string GetAddress<T, TP>(Expression<Func<T, TP>> expr)
        {
            return GetKey(expr)?.ToString();
        }
        
        public static List<string> GetPropertySelectionPath<T, TP>(Expression<Func<T, TP>> expr)
        {
            MemberExpression memberExpression;
            
            switch (expr.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    memberExpression = ((expr.Body is UnaryExpression ue) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    memberExpression = expr.Body as MemberExpression;
                    break;
            }

            var nameList = new List<string>();

            while (memberExpression != null)
            {
                string propertyName = memberExpression.Member.Name;

                nameList.Add(propertyName);

                var parentExpression = memberExpression.Expression;


                if (parentExpression == null)
                {
                    break;
                }
                if (parentExpression is MemberExpression memberParent)
                {
                    memberExpression = memberParent;
                }else if (parentExpression is MethodCallExpression callParent)
                {
                    nameList.Add("[-1]");

                    break;
                }
                else 
                {
                    nameList.Add(parentExpression.Type.Name);

                    break;
                }
            }

            
            return nameList;
        }


        public static FieldKey GetKey<T, TP>(Expression<Func<T, TP>> expr)
        {
            var nameList = GetPropertySelectionPath(expr);
            
            if (nameList.Count < 1)
            {
                return null;
            }
            
            nameList.RemoveAt(nameList.Count-1);
            
            nameList.Reverse();


            var evaluator = new ObjectEvaluator(typeof(T));

            var node = evaluator.RootNode;

            foreach (var name in nameList)
            {
                node = node.GetChildren().FirstOrDefault(c => c.Name == name);

                if (node == null)
                {
                    return null;
                }

                var currentKey = evaluator.Map.FieldKeyByNode(node);

                if (node.IsCollection)
                {
                    node = node.GetChildren()[0];
                }
            }

            var key = evaluator.Map.FieldKeyByNode(node);

            return key;
        }
    }
}