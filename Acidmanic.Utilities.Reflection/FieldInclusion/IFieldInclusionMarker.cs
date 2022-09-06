using System;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Acidmanic.Utilities.Reflection.FieldInclusion
{
    public interface IFieldInclusionMarker<TModel>
    {

        void Clear();

        IFieldInclusionMarker<TModel> Exclude<TProperty>(Expression<Func<TModel, TProperty>> propertySelector);
        
        IFieldInclusionMarker<TModel> Exclude(FieldKey key);
        
        IFieldInclusionMarker<TModel> Exclude(string address);
        
        IFieldInclusionMarker<TModel> UnExclude<TProperty>(Expression<Func<TModel, TProperty>> propertySelector);
        
        IFieldInclusionMarker<TModel> UnExclude(FieldKey key);
        
        IFieldInclusionMarker<TModel> UnExclude(string address);
        
        
        
        IFieldInclusionMarker<TModel> Rename<TProperty>(Expression<Func<TModel, TProperty>> propertySelector, string newName);
            
        IFieldInclusionMarker<TModel> Rename(FieldKey key, string newName);
        
        IFieldInclusionMarker<TModel> Rename(string address, string newName);

        IFieldInclusionMarker<TModel> UnRename<TProperty>(Expression<Func<TModel, TProperty>> propertySelector);
        
        IFieldInclusionMarker<TModel> UnRename(FieldKey key);
        
        IFieldInclusionMarker<TModel> UnRename(string address);
        

    }
}