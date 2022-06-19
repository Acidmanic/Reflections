using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.DataSource;
using Acidmanic.Utilities.Reflection.ObjectTree.Evaluators;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Reflection.Sets;

namespace Acidmanic.Utilities.Reflection.ObjectTree
{
    public class ObjectEvaluator
    {
        private readonly AccessNode _rootNode;
        private readonly object _rootObject;
        private readonly AddressKeyNodeMap _leavesMap;
        private readonly AddressKeyNodeMap _nodesMap;


        public AccessNode RootNode => _rootNode;

        public object RootObject => _rootObject;

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

        private object ReadLeaf(AccessNode leaf, object rootObject, int[] indexMap = null)
        {
            if (leaf.IsRoot)
            {
                return rootObject;
            }

            var parentObject = ReadLeaf(leaf.Parent, rootObject, indexMap);

            if (leaf.Evaluator is CollectableEvaluator cEvaluator)
            {
                return cEvaluator.Read(parentObject, indexMap);
            }

            return leaf.Evaluator.Read(parentObject);
        }

        private void WriteLeaf(AccessNode leaf, object rootObject, object value, int[] indexMap = null)
        {
            if (leaf.IsRoot)
            {
                Console.WriteLine("What the hell??");
                return;
            }

            var parentNode = leaf.Parent;

            var parentObject = ReadLeaf(parentNode, rootObject, indexMap);

            if (parentObject == null)
            {
                parentObject = new TypeAnalyzer().CreateObject(parentNode.Type, true);

                WriteLeaf(parentNode, rootObject, parentObject, indexMap);
            }

            if (leaf.Evaluator is CollectableEvaluator cEvaluator)
            {
                cEvaluator.Write(parentObject, indexMap, value);
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

                var indexMap = key.GetIndexMap();

                if (indexMap.Length > 0)
                {
                    return ReadLeaf(leaf, _rootObject, indexMap);
                }

                return ReadLeaf(leaf, _rootObject);
            }

            return null;
        }

        public void Write(FieldKey key, object value)
        {
            int keyIndex = _nodesMap.IndexOfKey(key, FieldKeyComparisons.IgnoreAllIndexes);

            if (keyIndex > -1)
            {
                var leaf = _nodesMap.Nodes[keyIndex];

                var indexMap = key.GetIndexMap();

                if (indexMap.Length > 0)
                {
                    WriteLeaf(leaf, _rootObject, value, indexMap);
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

        public void Write(string address, object value)
        {
            var key = FieldKey.Parse(address);

            if (key != null)
            {
                Write(key, value);
            }
        }

        public List<DataPoint> ToStandardFlatData()
        {
            var standardFlatData = new List<DataPoint>();

            var rootKey = new FieldKey().Append(new Segment(_rootNode.Name));

            EnumerateStandardLeaves(_rootNode, rootKey, standardFlatData);

            return standardFlatData;
        }


        private void EnumerateStandardLeaves(AccessNode node, FieldKey nodeKey, List<DataPoint> result)
        {
            if (node.IsLeaf)
            {
                var value = Read(nodeKey);

                result.Add(new DataPoint
                {
                    Identifier = nodeKey.ToString(),
                    Value = value
                });
            }
            else
            {
                if (node.IsCollection)
                {
                    var collectionObject = Read(nodeKey);

                    if (collectionObject != null && collectionObject is ICollection coll)
                    {
                        var collection = new CollectionCollection(coll);

                        int index = 0;

                        var collectableChildNode = node.GetChildren().First();

                        var collectableChildName = collectableChildNode.Name;

                        foreach (var item in collection)
                        {
                            var childSegment = Segment.Parse(collectableChildName);

                            childSegment.Index = index;

                            var childKey = nodeKey.Append(childSegment);

                            index += 1;

                            EnumerateStandardLeaves(collectableChildNode, childKey, result);
                        }
                    }
                }
                else
                {
                    // Normal Middle node (Field node)

                    var children = node.GetChildren();

                    foreach (var child in children)
                    {
                        var childKey = nodeKey.Append(new Segment(child.Name));

                        EnumerateStandardLeaves(child, childKey, result);
                    }
                }
            }
        }
    }
}