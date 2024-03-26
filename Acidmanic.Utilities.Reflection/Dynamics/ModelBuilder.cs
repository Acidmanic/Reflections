using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Acidmanic.Utilities.Reflection.Dynamics
{
    public class ModelBuilder
    {
        private readonly TypeBuilder _typeBuilder;

        
        private readonly  Dictionary<string,object> _initialValuesByPropertyName;

        public ModelBuilder(string typeName, string assemblyName = "DynamicModelsAssembly",
            string moduleName = "DynamicModelsModule")
        {
            var assemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);

            var moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);

            _typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Class);

            _initialValuesByPropertyName = new Dictionary<string, object>();
        }


        public ModelBuilder AddProperty(string name, Type type, object value)
        {
            AddProperty(_typeBuilder, name, type);
            
            _initialValuesByPropertyName.Add(name,value);

            return this;
        }
        
        public ModelBuilder AddProperty(string name, Type type)
        {
            AddProperty(_typeBuilder, name, type);
            
            return this;
        }

        private void AddProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            var propertyBuilder =
                typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, new Type[] { });

            var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            var getter =
                typeBuilder.DefineMethod("Get" + propertyName + "Accessor",
                    MethodAttributes.Public,
                    propertyType,
                    Type.EmptyTypes);

            var setter =
                typeBuilder.DefineMethod("Set" + propertyName + "Accessor",
                    MethodAttributes.Public,
                    null,
                    new Type[] {propertyType});


            var getterIlGen = getter.GetILGenerator();
            var setterIlGen = setter.GetILGenerator();


            getterIlGen.Emit(OpCodes.Ldarg_0);
            getterIlGen.Emit(OpCodes.Ldfld, fieldBuilder);
            getterIlGen.Emit(OpCodes.Ret);

            setterIlGen.Emit(OpCodes.Ldarg_0);
            setterIlGen.Emit(OpCodes.Ldarg_1);
            setterIlGen.Emit(OpCodes.Stfld, fieldBuilder);
            setterIlGen.Emit(OpCodes.Ret);


            propertyBuilder.SetSetMethod(setter);
            propertyBuilder.SetGetMethod(getter);
        }


        public Type Build()
        {
            return _typeBuilder.CreateType();
        }


        public object BuildObject()
        {
            var type = Build();

            var owner =  new ObjectInstantiator().CreateObject(type);

            foreach (var initializer in _initialValuesByPropertyName)
            {
                var property = type.GetProperty(initializer.Key);

                if (property != null)
                {
                    property.SetValue(owner,initializer.Value);
                }
            }

            return owner;
        }
    }
}