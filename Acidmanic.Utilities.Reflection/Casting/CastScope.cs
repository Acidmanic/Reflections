using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Utilities;

namespace Acidmanic.Utilities.Reflection.Casting
{
    public class CastScope : IDisposable
    {
        private static readonly DoubleKeyDictionary<Type, Type, ICast> AvailableCasts =
            new DoubleKeyDictionary<Type, Type, ICast>();

        private static readonly object AccessLock = new object();

        private readonly IEnumerable<ICast> _casts;

        private CastScope(IEnumerable<ICast> casts)
        {
            _casts = casts;

            lock (AccessLock)
            {
                foreach (var cast in _casts)
                {
                    AvailableCasts.Append(cast);
                }
            }
        }

        public void Dispose()
        {
            lock (AccessLock)
            {
                foreach (var cast in _casts)
                {
                    AvailableCasts.Remove(cast);
                }
            }
        }

        internal static DoubleKeyDictionary<Type, Type, ICast> GetAvailableCasts()
        {
            lock (AccessLock)
            {
                return AvailableCasts;
            }
        }

        public static CastScope Create(IEnumerable<ICast> casts)
        {
            return new CastScope(casts);
        }
        
        public static CastScope Create(params ICast[] casts)
        {
            return new CastScope(casts);
        }
    }
}