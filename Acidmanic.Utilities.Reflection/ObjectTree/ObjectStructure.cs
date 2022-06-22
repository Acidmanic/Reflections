using System;
using System.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree.Evaluators;
using Acidmanic.Utilities.Reflection.ObjectTree.ObjectTreeNaming;
using Acidmanic.Utilities.Reflection.Sets;

namespace Acidmanic.Utilities.Reflection.ObjectTree
{
    public class ObjectStructure
    {

        public static AccessNode CreateStructure<TModel>(bool fullTree)
        {
            var type = typeof(TModel);

            return CreateStructure(type, fullTree);
        }
        
        
        public static AccessNode CreateStructure(Type type, bool fullTree)
        {
            var evaluator = new RootObjectEvaluator();

            var rootName = new AccessNodeNameReference().GetNameForRoot(type);

            var node = new AccessNode(rootName, type, evaluator, false, false, 0);

            AppendChildren(node, fullTree);

            return node;
        }


        private static void AppendChildren(AccessNode node, bool fullTree)
        {
            var type = node.Type;

            var isCollection = TypeCheck.IsCollection(type);

            if (isCollection)
            {
                AppendCollectionChildren(node, fullTree);

                return;
            }

            var isReference = TypeCheck.IsReferenceType(type);
            // If The node is a leaf, we are already done.  
            // otherwise its time to add children
            if (isReference)
            {
                AppendFieldChildren(node, fullTree);
            }
        }


        private static void AppendCollectionChildren(AccessNode node, bool fullTree)
        {
            var evaluator = new CollectableEvaluator();

            var type = TypeCheck.GetElementType(node.Type);

            var childName = new AccessNodeNameReference().GetNameForCollectable(type);

            var child = new AccessNode(childName, type, evaluator, false, false, node.Depth + 1);

            evaluator.SetNodeDepthInformer(() => child.Depth);
            
            
            node.Add(child);

            AppendChildren(child, fullTree);
        }


        private static void AppendFieldChildren(AccessNode node, bool fullTree)
        {
            var type = node.Type;

            int depth = node.Depth;

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var pType = property.PropertyType;

                if (!TypeCheck.IsReferenceType(pType) || fullTree)
                {
                    var evaluator = new FieldEvaluator(property);

                    var isUnique = FieldInfo.IsUnique(property);

                    var isAuto = FieldInfo.IsAutogenerated(property);

                    var childName = new AccessNodeNameReference().GetNameForField(property);

                    var child = new AccessNode(childName, pType, evaluator, isUnique, isAuto, depth + 1);

                    if (fullTree)
                    {
                        AppendChildren(child, true);
                    }

                    node.Add(child);
                }
            }
        }
    }
}