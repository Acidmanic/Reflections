



using System;

namespace Acidmanic.Utilities.Reflection{



    internal class MetaType{


        public Type Type {get;set;}

        public Func<Object> Instanciator{get;set;}


        public MetaType(Type type){

            Type = type;

            Instanciator = () => Make(type);
        }

        public MetaType(Type type,Func<Object> instanciator){
            Type = type;

            Instanciator = instanciator;
        }


        private Object Make(Type type){
            try
            {
                var cons = type.GetConstructor(new Type[]{});

                return cons.Invoke(new Object[]{});
            }
            catch (System.Exception)
            {      }
            return null;
        }
    }
}