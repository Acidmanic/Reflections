using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.Dynamics;
using CorePluralizer.Extensions;

namespace Acidmanic.Utilities.Reflection
{
    public class EnumerableDynamicWrapper<TModel>
    {
        private readonly Action<IEnumerable<TModel>,object> _setter;
        private readonly Func<Object> _instantiate;
        public EnumerableDynamicWrapper():this(typeof(TModel).Name.ToPlural())
        {
        }
        
        public EnumerableDynamicWrapper(string name)
        {
            var type = typeof(TModel);
            
            var builder = new ModelBuilder("ResponseWrap").AddProperty(name, typeof(List<TModel>));

            var wrappedType = builder.Build();

            var property = wrappedType.GetProperties()
                .FirstOrDefault(p => p.Name == name);
            
            _instantiate = () => builder.BuildObject();

            
            _setter = (data, obj) =>
            {
                var listedData = new List<TModel>();
            
                listedData.AddRange(data);
                
               property?.SetValue(obj,listedData);
            };
               
        }

        public object Wrap(IEnumerable<TModel> data)
        {
            var wrapped = _instantiate();

            _setter(data, wrapped);

            return wrapped;
        }
    }
}