using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Acidmanic.Utilities.Reflection.ObjectTree
{
    public class ObjectEvaluator
    {
        private readonly AccessNode _rootNode;
        private readonly object _rootObject;
        private readonly Dictionary<string, FieldKey> _keysByAddress;
        private readonly Dictionary<string, AccessNode> _leavesByAddress;

        private ObjectEvaluator(AccessNode rootNode, object rootObject)
        {
            _rootNode = rootNode;
            _rootObject = rootObject;

            _keysByAddress = new Dictionary<string, FieldKey>();

            _leavesByAddress = new Dictionary<string, AccessNode>();


            var leaves = _rootNode.EnumerateLeavesBelow();

            foreach (var leaf in leaves)
            {
                var address = leaf.GetFullName();

                _leavesByAddress.Add(address, leaf);

                _keysByAddress.Add(address, FieldKey.Parse(address));
            }
        }


        public ObjectEvaluator(Type type) : this(
            ObjectStructure.CreateStructure(type, true),
            new TypeAnalyzer().CreateObject(type, true))
        {
        }

        public ObjectEvaluator(object rootObject) :
            this(ObjectStructure.CreateStructure(rootObject.GetType(), true), rootObject)
        {
        }

        private object ReadLeaf(AccessNode leaf, object rootObject)
        {
            if (leaf.IsRoot)
            {
                return rootObject;
            }

            var parentObject = ReadLeaf(leaf.Parent, rootObject);

            return leaf.Evaluator.Read(parentObject);
        }

        private void WriteLeaf(AccessNode leaf, object rootObject, object value)
        {
            if (leaf.IsRoot)
            {
                Console.WriteLine("What the hell??");
                return;
            }

            var parentNode = leaf.Parent;

            var parentObject = ReadLeaf(parentNode, rootObject);

            if (parentObject == null)
            {
                parentObject = new TypeAnalyzer().CreateObject(parentNode.Type, true);

                WriteLeaf(parentNode, rootObject, parentObject);
            }

            leaf.Evaluator.Write(parentObject, value);
        }


        public object Read(FieldKey key)
        {
            var address = key.ToString();

            return Read(address);
        }

        public void Write(FieldKey key, object value)
        {
            var address = key.ToString();

            Write(address, value);
        }

        public object Read(string address)
        {
            if (_leavesByAddress.ContainsKey(address))
            {
                var leaf = _leavesByAddress[address];

                return ReadLeaf(leaf, _rootObject);
            }

            return null;
        }

        public void Write(string address, object value)
        {
            if (_leavesByAddress.ContainsKey(address))
            {
                var leaf = _leavesByAddress[address];

                WriteLeaf(leaf, _rootObject, value);
            }
        }
    }
}