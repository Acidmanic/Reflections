using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.DataSource;
using Acidmanic.Utilities.Reflection.ObjectTree.Evaluators;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
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

        public AddressKeyNodeMap Map => _nodesMap;

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


        private object ReadLeaf(AccessNode leaf, object rootObject, int[] indexMap = null)
        {
            if (leaf.IsRoot)
            {
                return rootObject;
            }

            var parentObject = ReadLeaf(leaf.Parent, rootObject, indexMap);

            if (parentObject == null)
            {
                return null;
            }

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

        public TModel As<TModel>()
        {
            if (_rootObject != null && _rootObject is TModel model)
            {
                return model;
            }

            return default;
        }

        public void LoadStandardData(Record record)
        {
            record.ForEach(dp => Write(dp.Identifier, dp.Value));
        }

        public Record ToStandardFlatData()
        {
            var standardFlatData = new Record();

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