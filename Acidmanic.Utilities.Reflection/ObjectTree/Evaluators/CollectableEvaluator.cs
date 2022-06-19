using System;
using System.Collections;
using System.Linq;
using Acidmanic.Utilities.Reflection.Sets;

namespace Acidmanic.Utilities.Reflection.ObjectTree.Evaluators
{
    public class CollectableEvaluator : IEvaluator
    {
        public object Read(object parentObject)
        {
            var collection = Wrap(parentObject);

            if (collection.Count == 0)
            {
                collection.Add(new TypeAnalyzer().CreateObject(collection.ElementType, true));
            }

            return collection.Last();
        }

        public object Read(object parentObject, int index)
        {
            if (index < 0)
            {
                return Read(parentObject);
            }

            var collection = Wrap(parentObject);

            while (collection.Count <= index)
            {
                AddInstance(collection);
            }

            return collection.ToList()[index];
        }

        public void Write(object parentObject, int index, object value)
        {
            if (index < 0)
            {
                Write(parentObject, value);

                return;
            }

            var collection = Wrap(parentObject);

            while (collection.Count <= index)
            {
                AddInstance(collection);
            }

            collection.ToList()[index] = value;
        }

        private void AddInstance(CollectionCollection collection)
        {
            collection.Add(new TypeAnalyzer().CreateObject(collection.ElementType, true));
        }

        public void Write(object parentObject, object value)
        {
            var collection = Wrap(parentObject);

            if (value == null)
            {
                collection.Add(null);
                return;
            }

            if (value.GetType() != collection.ElementType)
            {
                throw new Exception($"Type mismatch: Adding an element of type " +
                                    $"{value.GetType().Name} instead to a collection of {collection.ElementType.Name}");
            }

            collection.Add(value);
        }

        private CollectionCollection Wrap(object collectionObject)
        {
            if (collectionObject == null)
            {
                throw new Exception("Collection object can not be null");
            }

            if (!TypeCheck.IsCollection(collectionObject.GetType()))
            {
                throw new Exception(
                    $"Received a {collectionObject.GetType().Name} Instead where a collection was expected"
                );
            }

            return new CollectionCollection((ICollection) collectionObject);
        }

        public int Count(object parentObject)
        {
            var collection = Wrap(parentObject);

            return collection.Count;
        }
    }
}