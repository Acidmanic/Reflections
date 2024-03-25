using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Acidmanic.Utilities.Reflection.FieldInclusion
{
    public class FiledManipulationMarker : IFieldInclusion, IFieldInclusionMarker
    {
        private readonly List<FieldKey> _excludedNames;
        private readonly Dictionary<FieldKey, string> _renames;

        public FiledManipulationMarker()
        {
            _excludedNames = new List<FieldKey>();
            _renames = new Dictionary<FieldKey, string>();
        }


        public IFieldInclusionMarker Exclude<TModel,TProperty>(Expression<Func<TModel, TProperty>> propertySelector)
        {
            var key = MemberOwnerUtilities.GetKey(propertySelector);
            
            return Exclude(key);
        }

        public IFieldInclusionMarker Exclude(FieldKey key)
        {
            if (key != null)
            {
                _excludedNames.Add(key);   
            }
            return this;
        }

        public IFieldInclusionMarker Exclude(string address)
        {
            var key = AddressToKey(address);
            
            return Exclude(key);
        }

        public IFieldInclusionMarker UnExclude<TModel,TProperty>(Expression<Func<TModel, TProperty>> propertySelector)
        {
            var key = MemberOwnerUtilities.GetKey(propertySelector);
            
            return UnExclude(key);
        }

        public IFieldInclusionMarker UnExclude(FieldKey key)
        {
            if (key!=null && _excludedNames.Contains(key))
            {
                _excludedNames.Remove(key);
            }
            return this;
        }

        public IFieldInclusionMarker UnExclude(string address)
        {
            var key = AddressToKey(address);

            return UnExclude(key);
        }

        public IFieldInclusionMarker Rename<TModel,TProperty>(Expression<Func<TModel, TProperty>> propertySelector, string newName)
        {
            var key = MemberOwnerUtilities.GetKey(propertySelector);
            
            return Rename(key, newName);
        }

        public IFieldInclusionMarker Rename(FieldKey key, string newName)
        {
            if (key != null)
            {
                _renames.Add(key, newName);    
            }
            return this;
        }

        public IFieldInclusionMarker Rename(string address, string newName)
        {
            var key = AddressToKey(address);

            return Rename(key, newName);
        }

        public IFieldInclusionMarker UnRename<TModel,TProperty>(Expression<Func<TModel, TProperty>> propertySelector)
        {
            var key = MemberOwnerUtilities.GetKey(propertySelector);
            
            return UnRename(key);

        }

        public IFieldInclusionMarker UnRename(FieldKey key)
        {
            if (key != null && _renames.ContainsKey(key))
            {
                _renames.Remove(key);
            }
            return this;
        }

        public IFieldInclusionMarker UnRename(string address)
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


        public bool IsIncluded<TM,TP>(Expression<Func<TM, TP>> expr)
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

        public string GetPracticalName<TM,TProperty>(Expression<Func<TM, TProperty>> propertySelector)
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