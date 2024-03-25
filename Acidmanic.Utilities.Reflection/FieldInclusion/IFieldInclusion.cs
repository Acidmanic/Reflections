using System;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Acidmanic.Utilities.Reflection.FieldInclusion
{
    public interface IFieldInclusion
    {

        bool IsIncluded(FieldKey key);

        bool IsIncluded(string address);

        bool IsIncluded<TModel,TProperty>(Expression<Func<TModel, TProperty>> propertySelector);

        string GetPracticalName(FieldKey key);

        string GetPracticalName(string address);
        
        string GetPracticalName<TModel,TProperty>(Expression<Func<TModel, TProperty>> propertySelector);
        
    }
    
}