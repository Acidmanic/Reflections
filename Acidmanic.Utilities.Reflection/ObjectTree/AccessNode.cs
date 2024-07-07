using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.ObjectTree.Evaluators;

namespace Acidmanic.Utilities.Reflection.ObjectTree
{
    public class AccessNode
    {
        public string Name { get; }

        public Type Type { get; }
        
        public Type? AlteredType { get; }
        
        public AccessNode Parent { get; private set; }

        protected List<AccessNode> Children { get; }

        public List<Attribute> PropertyAttributes { get; } = new();

        public bool IsLeaf => Children.Count == 0;

        public bool IsRoot => Parent == null;

        public bool IsUnique { get; }

        public bool IsAutoValued { get; }
        
        public bool IsAlteredType { get; }

        public bool IsCollection { get; }

        public IEvaluator Evaluator { get; }


        public AccessNode(string name, Type type, IEvaluator evaluator, bool isUnique, bool isAutoValued, int depth)
        {
            Name = name;

            IsCollection = TypeCheck.IsCollection(type);

            Type = type;

            Parent = null;

            Children = new List<AccessNode>();

            IsUnique = isUnique;

            IsAutoValued = isAutoValued;

            AlteredType = GetAlteredType(type);

            IsAlteredType = AlteredType != null;

            Depth = depth;

            Evaluator = evaluator;
        }

        public void Add(AccessNode child)
        {
            child.Parent = this;

            Children.Add(child);
        }

        public string GetFullName()
        {
            if (IsRoot)
            {
                return Name;
            }

            return Parent.GetFullName() + "." + Name;
        }


        public List<AccessNode> EnumerateLeavesBelow()
        {
            var result = new List<AccessNode>();

            EnumerateLeavesBelow(result);

            return result;
        }

        private Type? GetAlteredType(Type type)
        {
            var attribute = type.GetCustomAttribute<AlteredTypeAttribute>();

            if (attribute is { } a)
            {
                return a.AlternativeType;
            }

            return null;
        }
        
        private void EnumerateLeavesBelow(ICollection<AccessNode> result)
        {
            if (IsLeaf && !IsRoot)
            {
                result.Add(this);
            }
            else
            {
                foreach (var child in Children)
                {
                    child.EnumerateLeavesBelow(result);
                }
            }
        }

        public Type ElementType
        {
            get
            {
                if (TypeCheck.Implements<ICollection>(Type))
                {
                    return Type.GenericTypeArguments[0];
                }

                if (TypeCheck.Extends<Array>(Type))
                {
                    return Type.GetElementType();
                }

                return null;
            }
        }

        public List<AccessNode> GetChildren()
        {
            return new List<AccessNode>(Children);
        }

        public AccessNode GetTopLevelNode()
        {
            return GetTopLevelNode(this);
        }

        public List<AccessNode> GetDirectLeaves(bool alteredTypesAsLeaf=false)
        {
            var directLeaves = new List<AccessNode>();

            foreach (var child in Children)
            {
                if (child.IsLeaf || (alteredTypesAsLeaf && child.IsAlteredType && !TypeCheck.IsReferenceType(child.AlteredType)))
                {
                    directLeaves.Add(child);
                }
            }

            return directLeaves;
        }

        private AccessNode GetTopLevelNode(AccessNode node)
        {
            if (node.IsRoot)
            {
                if (node.IsCollection)
                {
                    return null;
                }

                return node;
            }

            if (IsTopLevel(node.Parent))
            {
                return node.Parent;
            }

            return GetTopLevelNode(node.Parent);
        }


        private bool IsTopLevel(AccessNode node)
        {
            if (!node.IsRoot)
            {
                return node.Parent.IsCollection;
            }

            return !node.IsCollection;
        }

        public bool IsCollectable
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.IsCollection;
                }

                return false;
            }
        }

        public override string ToString()
        {
            return GetFullName();
        }


        public int Depth { get; protected set; }
    }
}