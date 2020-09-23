using System.Collections.Generic;

namespace Access.Auth.Service.Business.Extension
{
    public static class Extension
    {
        public static V GetOrDefault<K, V>(this Dictionary<K, V> dict, K key) where V : class
        {
            V val;
            dict.TryGetValue(key, out val);
            return val;
        }
    }
}
