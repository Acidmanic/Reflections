using System;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Acidmanic.Utilities.Reflection.FieldInclusion
{
    public interface IFieldInclusion<TModel>
    {

        bool IsIncluded(FieldKey key);
       
        bool IsIncluded(string address);

        bool IsIncluded<TProperty>(Expression<Func<TModel, TProperty>> propertySelector);

        string GetPracticalName(FieldKey key);
        
        string GetPracticalName(string address);
        
        string GetPracticalName<TProperty>(Expression<Func<TModel, TProperty>> propertySelector);
        
    }
}