using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.DataSource;
using Acidmanic.Utilities.Reflection.Extensions;
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

        public ObjectEvaluator(Type type) : this(
            ObjectStructure.CreateStructure(type, true),
            new ObjectInstantiator().CreateObject(type, true))
        {
        }

        public ObjectEvaluator(object rootObject) :
            this(ObjectStructure.CreateStructure(rootObject.GetType(), true), rootObject)
        {
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

            //Read Arrays here i guess

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
                parentObject = new ObjectInstantiator().CreateObject(parentNode.Type, true);

                WriteLeaf(parentNode, rootObject, parentObject, indexMap);
            }

            if (leaf.Evaluator is CollectableEvaluator cEvaluator)
            {
                cEvaluator.Write(parentObject, indexMap, value);

                return;
            }

            // Write array evaluator here i guess

            leaf.Evaluator.Write(parentObject, value);
        }


        public object Read(FieldKey key,bool castAltered = false)
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

                    return;
                }

                WriteLeaf(leaf, _rootObject, value);
            }
        }

        public object Read(string address,bool castAltered = false)
        {
            var key = FieldKey.Parse(address);

            if (key != null)
            {
                return Read(key,castAltered);
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
            return ToStandardFlatData(o => { });
        }

        [Obsolete("This method will be removed in later releases. please use other overloads of this method.")]
        public Record ToStandardFlatData(bool directLeavesOnly = false, bool excludeNulls = false)
        {
            return ToStandardFlatData();
        }

        public Record ToStandardFlatData(Action<IStandardConversionOptionsBuilder> options)
        {
            var opt = new StandardConversionOptions();

            options(opt);

            return ToStandardFlatData(opt);
        }


        private Record ToStandardFlatData(StandardConversionOptions options)
        {
            return options.DirectLeaves
                ? GetStandardFlatDataForDirectLeaves(options.CastAltered)
                : GetStandardFlatDataFullTree(options.ExcludeNullValues,options.CastAltered);
        }

        private Record GetStandardFlatDataFullTree(bool excludeNulls,bool castAltered)
        {
            var standardFlatData = new Record();

            var rootKey = new FieldKey().Append(new Segment(_rootNode.Name));

            EnumerateStandardLeaves(_rootNode, rootKey, excludeNulls, castAltered,standardFlatData);

            return standardFlatData;
        }

        private Record GetStandardFlatDataForDirectLeaves(bool castAltered)
        {
            var standardFlatData = new Record();

            var rootKey = new FieldKey().Append(new Segment(_rootNode.Name));

            if (_rootNode.IsCollection)
            {
                if (_rootObject != null && _rootObject is IList list)
                {
                    var wrapped = new ListWrap(list);

                    var collectableChildNode = _rootNode.GetChildren().First();

                    var collectableChildName = collectableChildNode.Name;

                    for (int i = 0; i < wrapped.Count; i++)
                    {
                        var value = wrapped[i];

                        var childSegment = Segment.Parse(collectableChildName);

                        childSegment.Index = i;

                        var childKey = rootKey.Append(childSegment);

                        value = CastChecked(value, collectableChildNode.Type, castAltered);

                        standardFlatData.Add(childKey.ToString(), value);
                    }
                }
            }
            else
            {
                var leaves = _rootNode.GetDirectLeaves();

                foreach (var leaf in leaves)
                {
                    var childKey = rootKey.Append(new Segment(leaf.Name));

                    var value = leaf.Evaluator.Read(_rootObject);

                    value = CastChecked(value, leaf.Type, castAltered);
                    
                    standardFlatData.Add(childKey.ToString(), value);
                }
            }

            return standardFlatData;
        }

        private object CastChecked(object value, Type type, bool cast)
        {
            if (value == null)
            {
                return null;
            }
            if (cast)
            {
                var targetType = type.GetAlteredOrOriginal();

                return value.CastTo(targetType);
            }

            return value;
        }
        
        private void EnumerateStandardLeaves(AccessNode node, FieldKey nodeKey, bool excludeNulls,
            bool castAltered, List<DataPoint> result)
        {
            if (node.IsLeaf)
            {
                var value = Read(nodeKey,castAltered);

                result.Add(new DataPoint
                {
                    Identifier = nodeKey.ToString(),
                    Value = value
                });
            }
            else
            {
                if (!excludeNulls || (Read(nodeKey,castAltered) != null))
                {
                    if (node.IsCollection)
                    {
                        var collectionObject = Read(nodeKey,castAltered);

                        if (collectionObject != null && collectionObject is IList list)
                        {
                            var collection = new ListWrap(list);

                            int index = 0;

                            var collectableChildNode = node.GetChildren().First();

                            var collectableChildName = collectableChildNode.Name;

                            foreach (var item in collection)
                            {
                                var childSegment = Segment.Parse(collectableChildName);

                                childSegment.Index = index;

                                var childKey = nodeKey.Append(childSegment);

                                index += 1;

                                EnumerateStandardLeaves(collectableChildNode, childKey, excludeNulls,castAltered, result);
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

                            EnumerateStandardLeaves(child, childKey, excludeNulls,castAltered, result);
                        }
                    }
                }
            }
        }


        public AccessNode FindCorrespondingNode(AccessNode foreignNode)
        {
            var fullName = foreignNode.GetFullName();

            return Map.Nodes.FirstOrDefault(n => n.GetFullName() == fullName);
        }

        public FieldKey FindCorrespondingKey(AccessNode foreignNode)
        {
            var fullName = foreignNode.GetFullName();

            return Map.Nodes.Where(n => n.GetFullName() == fullName)
                .Select(n => Map.FieldKeyByNode(n))
                .FirstOrDefault();
        }
    }
}