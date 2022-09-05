using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Acidmanic.Utilities.Reflection.FieldInclusion
{
    public class FiledManipulationMarker<TModel> : IFieldInclusion<TModel>, IFieldInclusionMarker<TModel>
    {
        private readonly List<FieldKey> _excludedNames;
        private readonly Dictionary<FieldKey, string> _renames;

        public FiledManipulationMarker()
        {
            _excludedNames = new List<FieldKey>();
            _renames = new Dictionary<FieldKey, string>();
        }


        public IFieldInclusionMarker<TModel> Exclude<TProperty>(Expression<Func<TModel, TProperty>> propertySelector)
        {
            var key = MemberOwnerUtilities.GetKey(propertySelector);

            return Exclude(key);
        }

        public IFieldInclusionMarker<TModel> Exclude(FieldKey key)
        {
            if (key != null)
            {
                _excludedNames.Add(key);   
            }
            return this;
        }

        public IFieldInclusionMarker<TModel> Exclude(string address)
        {
            var key = AddressToKey(address);

            return Exclude(key);
        }

        public IFieldInclusionMarker<TModel> UnExclude<TProperty>(
            Expression<Func<TModel, TProperty>> propertySelector)
        {
            var key = MemberOwnerUtilities.GetKey(propertySelector);

            return UnExclude(key);
        }

        public IFieldInclusionMarker<TModel> UnExclude(FieldKey key)
        {
            if (key!=null && _excludedNames.Contains(key))
            {
                _excludedNames.Remove(key);
            }

            return this;
        }

        public IFieldInclusionMarker<TModel> UnExclude(string address)
        {
            var key = AddressToKey(address);

            return UnExclude(key);
        }

        public IFieldInclusionMarker<TModel> Rename<TProperty>(Expression<Func<TModel, TProperty>> propertySelector,
            string newName)
        {
            var key = MemberOwnerUtilities.GetKey(propertySelector);

            return Rename(key, newName);
        }

        public IFieldInclusionMarker<TModel> Rename(FieldKey key, string newName)
        {
            if (key != null)
            {
                _renames.Add(key, newName);    
            }
            return this;
        }

        public IFieldInclusionMarker<TModel> Rename(string address, string newName)
        {
            var key = AddressToKey(address);

            return Rename(key, newName);
        }

        public IFieldInclusionMarker<TModel> UnRename<TProperty>(Expression<Func<TModel, TProperty>> propertySelector)
        {
            var key = MemberOwnerUtilities.GetKey(propertySelector);

            UnRename(key);

            return this;
        }

        public IFieldInclusionMarker<TModel> UnRename(FieldKey key)
        {
            if (key != null && _renames.ContainsKey(key))
            {
                _renames.Remove(key);
            }

            return this;
        }

        public IFieldInclusionMarker<TModel> UnRename(string address)
        {
            var key = AddressToKey(address);

            return UnRename(key);
        }


        public List<string> ExcludedNames()
        {
            return new List<string>(_excludedNames.Select(k => k.ToString()));
        }


        public bool IsIncluded(FieldKey key)
        {
            if (key == null)
            {
                return false;
            }
            return !_excludedNames.Contains(key);
        }

        public bool IsIncluded(string address)
        {
            var key = FieldKey.Parse(address);

            if (key == null)
            {
                return false;
            }

            return IsIncluded(key);
        }


        public bool IsIncluded<TP>(Expression<Func<TModel, TP>> expr)
        {
            var key = MemberOwnerUtilities.GetKey(expr);

            if (key == null)
            {
                return false;
            }

            return !_excludedNames.Contains(key);
        }

        public string GetPracticalName(FieldKey key)
        {
            if (key!=null && _renames.ContainsKey(key))
            {
                return _renames[key];
            }

            return null;
        }

        public string GetPracticalName(string address)
        {
            var key = FieldKey.Parse(address);

            if (key == null)
            {
                return address;
            }

            return key.ToString();
        }

        public string GetPracticalName<TProperty>(Expression<Func<TModel, TProperty>> propertySelector)
        {
            var key = MemberOwnerUtilities.GetKey(propertySelector);

            return GetPracticalName(key);
        }

        public void Clear()
        {
            _renames.Clear();
            _excludedNames.Clear();
        }


        private FieldKey AddressToKey(string address)
        {
            var key = FieldKey.Parse(address);

            if (key == null)
            {
                throw new Exception("You should enter a valid standard address of the field.");
            }

            return key;
        }
    }
}