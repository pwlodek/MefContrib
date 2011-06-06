using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MefContrib.Hosting.Isolation.Runtime
{
    public static class SerializationServices
    {

        public static List<RuntimeArgument> Serialize(object[] values)
        {
            var formatter = new BinaryFormatter();
            var list = new List<RuntimeArgument>();
            for (int index = 0; index < values.Length; index++)
            {
                var value = values[index];
                
                var argument = new RuntimeArgument();

                if (value != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        formatter.Serialize(memoryStream, value);
                        argument.Value = memoryStream.ToArray();
                    }
                }
                
                list.Add(argument);
            }

            return list;
        }

        public static List<object> Deserialize(List<RuntimeArgument> arguments)
        {
            var formatter = new BinaryFormatter();
            var list = new List<object>();
            for (int index = 0; index < arguments.Count; index++)
            {
                var argument = arguments[index];
                object value = null;

                if (argument.Value != null && argument.Value.Length > 0)
                {
                    using (var memoryStream = new MemoryStream(argument.Value))
                    {
                        memoryStream.Position = 0;
                        value = formatter.Deserialize(memoryStream);
                    }
                }

                list.Add(value);
            }

            return list;
        }
    }
}