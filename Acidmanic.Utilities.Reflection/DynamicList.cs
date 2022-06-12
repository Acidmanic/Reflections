using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acidmanic.Utilities.Reflection
{
    public class DynamicList
    {
        public DynamicList(Type type )
        {
            var listGenericType = typeof (List<>);

            Type = listGenericType.MakeGenericType(type);

            var ci = Type.GetConstructor(new Type[] {});

            List = ci?.Invoke(new object[] {});

            PerformAdd = Type.GetMethod("Add");
        }
        
        public object List { get;  }
        
        public Type Type { get; }
        
        private MethodInfo PerformAdd { get; }

        public void Add(object item)
        {
            PerformAdd.Invoke(List,new object[]{item});
        }

        public T Cast<T>()
        {
            return (T) List;
        }
    }
}