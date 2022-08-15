using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Reflection.Test.Functional.Models;

namespace Reflection.Test.Functional
{
    public class Tdd005PropertyName : TestBase
    {

        class A
        {
            public int Id { get; set; }
            
            public B B { get; set; }
        }

        class B
        {
            public int Id { get; set; }
        }
        
        public override void Main()
        {
            var expression = JustCreateTheShit<A,int>(a => a.B.Id);
             
            var memberExpression = (MemberExpression) expression.Body;

            var name = new MemberOwnerUtilities(new PluralDataOwnerNameProvider()).GetFieldName<A>(memberExpression,true);

            Console.WriteLine(name);
        }

        private Expression<Func<TOwner, TProp>> JustCreateTheShit<TOwner, TProp>(
            Expression<Func<TOwner, TProp>> expression)
        {
            return expression;
        }
    }
}