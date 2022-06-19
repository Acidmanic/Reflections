using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.DataSource;
using Acidmanic.Utilities.Reflection.ObjectTree.Evaluators;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Acidmanic.Utilities.Reflection.ObjectTree
{
    public class ObjectEvaluator
    {
        private readonly AccessNode _rootNode;
        private readonly object _rootObject;
        private readonly AddressKeyNodeMap _leavesMap;
        private readonly AddressKeyNodeMap _nodesMap;

        private ObjectEvaluator(AccessNode rootNode, object rootObject)
        {
            _rootNode = rootNode;
            _rootObject = rootObject;

            _nodesMap = new AddressKeyNodeMap();
            _leavesMap = new AddressKeyNodeMap();

            IndexNodes(_rootNode);
        }

        private void IndexNodes(AccessNode node)
        {
            var address = node.GetFullName();

            var key = FieldKey.Parse(address);

            _nodesMap.Add(node, key, address);

            if (node.IsLeaf)
            {
                _leavesMap.Add(node, key, address);
            }
            else
            {
                var children = node.GetChildren();

                children.ForEach(IndexNodes);
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

        private object ReadLeaf(AccessNode leaf, object rootObject, Stack<int> indexes = null)
        {
            if (leaf.IsRoot)
            {
                return rootObject;
            }

            var parentObject = ReadLeaf(leaf.Parent, rootObject);

            if (leaf.Evaluator is CollectableEvaluator cEvaluator)
            {
                return cEvaluator.Read(parentObject, indexes);
            }

            return leaf.Evaluator.Read(parentObject);
        }

        /// <summary>
        /// Returns the number of objects are on the same Leaf in the actual data.
        /// </summary>
        /// <returns>1 for non-collectable leaves, and the Count/Length of items in the collection,
        /// for collectable leaves.</returns>
        private int GetInstancesCountOnLeaf(AccessNode leaf, object rootObject)
        {
            if (leaf.Parent == null)
            {
                throw new Exception(
                    "Disfigured Node: A Collectable node can nut be root node too, It must have a Collection parent.");
            }

            if (leaf.Evaluator is CollectableEvaluator colEvaluator)
            {
                var parentObject = ReadLeaf(leaf.Parent, rootObject);

                return colEvaluator.Count(parentObject);
            }

            return ReadLeaf(leaf, rootObject) == null ? 0 : 1;
        }

        /// <summary>
        /// This Method will walk through the collection, if the given node is collectable.
        /// Otherwise nothing will happen.
        /// </summary>
        /// <returns> True, if the node was collectable, false otherwise </returns>
        private bool ForEach(AccessNode leaf, object rootObject, Action<int, object> expression)
        {
            if (leaf.Parent == null)
            {
                throw new Exception(
                    "Disfigured Node: A Collectable node can nut be root node too, It must have a Collection parent.");
            }

            if (leaf.Evaluator is CollectableEvaluator colEvaluator)
            {
                var parentObject = ReadLeaf(leaf.Parent, rootObject);

                var count = colEvaluator.Count(parentObject);

                for (int i = 0; i < count; i++)
                {
                    var objectAt = colEvaluator.Read(parentObject, i);

                    expression(i, objectAt);
                }

                return true;
            }

            return false;
        }

        private void WriteLeaf(AccessNode leaf, object rootObject, object value, int index = -1)
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

            if (leaf.Evaluator is CollectableEvaluator cEvaluator)
            {
                cEvaluator.Write(parentNode, index, value);
            }
            else
            {
                leaf.Evaluator.Write(parentObject, value);
            }
        }


        public object Read(FieldKey key)
        {
            int keyIndex = _nodesMap.IndexOfKey(key, FieldKeyComparisons.IgnoreAllIndexes);

            if (keyIndex > -1)
            {
                var leaf = _nodesMap.Nodes[keyIndex];

                var indexStack = key.GetIndexesStack();

                if (indexStack.Count > 0)
                {
                    return ReadLeaf(leaf, _rootObject, indexStack);
                }

                return ReadLeaf(leaf, _rootObject);
            }

            return null;
        }

        public void Write(FieldKey key, object value, FieldKeyComparisons comparison)
        {
            int keyIndex = _nodesMap.IndexOfKey(key, comparison);

            if (keyIndex > -1)
            {
                var leaf = _nodesMap.Nodes[keyIndex];

                if (key.Last().Indexed)
                {
                    WriteLeaf(leaf, _rootObject, value, key.Last().Index);
                }

                WriteLeaf(leaf, _rootObject, value);
            }
        }

        public object Read(string address)
        {
            var key = FieldKey.Parse(address);

            if (key != null)
            {
                return Read(key);
            }

            return null;
        }

        public void Write(string address, object value, FieldKeyComparisons comparison = FieldKeyComparisons.Strict)
        {
            var key = FieldKey.Parse(address);

            if (key != null)
            {
                Write(key, value, comparison);
            }
        }

        public List<DataPoint> ToStandardFlatData()
        {
            var standardFlatData = new List<DataPoint>();

            foreach (var leaf in _leavesMap.Nodes)
            {
                var key = _leavesMap.FieldKeyByNode(leaf);


                var leafWasCollectable = ForEach(leaf, _rootObject, (index, value) => { });

                if (!leafWasCollectable)
                {
                    var value = ReadLeaf(leaf, _rootObject);

                    var address = key.ToString();

                    standardFlatData.Add(new DataPoint
                    {
                        Identifier = address,
                        Value = value
                    });
                }
            }

            return standardFlatData;
        }
    }
}