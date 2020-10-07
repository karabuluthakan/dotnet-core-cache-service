using System.Text;
using MessagePack;
using StackExchange.Redis;

namespace CacheService.Extensions
{
    internal static class ObjectConvertExtensions
    {
        public static string Serialize(this object value)
        {
            return value is null
                ? default
                : Encoding.ASCII.GetString(MessagePackSerializer.Serialize(value));
        }

        public static byte[] ToByte(this object value)
        {
            return value is null
                ? default
                : MessagePackSerializer.Serialize(value);
        }

        public static T Cast<T>(this object value)
        {
            return value is null
                ? default
                : MessagePackSerializer.Deserialize<T>(MessagePackSerializer.Serialize(value));
        }

        public static T Deserialize<T>(this string value)
        {
            return string.IsNullOrEmpty(value)
                ? default
                : MessagePackSerializer.Deserialize<T>(Encoding.ASCII.GetBytes(value));
        }

        public static T Deserialize<T>(this RedisValue value)
        {
            return value.HasValue
                ? MessagePackSerializer.Deserialize<T>(Encoding.ASCII.GetBytes(value))
                : default;
        }
    }
}