using System;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Acidmanic.Utilities.Reflection.FieldInclusion
{
    public interface IFieldInclusionMarker
    {
        void Clear();

        IFieldInclusionMarker Exclude<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertySelector);

        IFieldInclusionMarker Exclude(FieldKey key);

        IFieldInclusionMarker Exclude(string address);

        IFieldInclusionMarker UnExclude<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertySelector);

        IFieldInclusionMarker UnExclude(FieldKey key);

        IFieldInclusionMarker UnExclude( string address);

        IFieldInclusionMarker Rename<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertySelector, string newName);

        IFieldInclusionMarker Rename(FieldKey key, string newName);

        IFieldInclusionMarker Rename(string address, string newName);

        IFieldInclusionMarker UnRename<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertySelector);

        IFieldInclusionMarker UnRename(FieldKey key);

        IFieldInclusionMarker UnRename(string address);
    }
}