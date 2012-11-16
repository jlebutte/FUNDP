using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Horaires.Tools
{
    public static class JsonHelper
    {
        public static string ToJson<T>(T instance)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var tempStream = new MemoryStream())
            {
                serializer.WriteObject(tempStream, instance);
                return Encoding.UTF8.GetString(tempStream.ToArray(), 0, Convert.ToInt32(tempStream.Length));
            }
        }
        public static T FromJson<T>(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var tempStream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                return (T)serializer.ReadObject(tempStream);
            }
        }
    }
}