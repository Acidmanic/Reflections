



using System;

namespace Acidmanic.Utilities.Reflection{



    public class MetaType{


        public Type Type {get;set;}

        public Func<Object> Instantiator{get;set;}


        public MetaType(Type type){

            Type = type;

            Instantiator = () => Make(type);
        }

        public MetaType(Type type,Func<Object> instantiator){
            Type = type;

            Instantiator = instantiator;
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