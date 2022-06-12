namespace Acidmanic.Utilities.Reflection.TypeCenter
{
    public class TypeCenterService : CachedTypeCenter
    {
        private static TypeCenterService _instance = null;

        private TypeCenterService()
        {
            CacheCurrent();
        }

        public static TypeCenterService Make()
        {
            var obj = new object();

            lock (obj)
            {
                if (_instance == null)
                {
                    _instance = new TypeCenterService();
                }

                _instance.ClearFilters();
            }

            return _instance;
        }
    }
}