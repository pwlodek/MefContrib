namespace MefContrib.Hosting.Isolation.Runtime.Remote
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class SerializationServices
    {
        private static readonly BinaryFormatter Formatter = new BinaryFormatter();

        public static InvokeReturnValue Serialize(object value)
        {
            var returnValue = new InvokeReturnValue();

            using (var memoryStream = new MemoryStream())
            {
                Formatter.Serialize(memoryStream, value);
                returnValue.Value = memoryStream.ToArray();
            }

            return returnValue;
        }

        public static object Deserialize(InvokeReturnValue value)
        {
            if (value.Value != null && value.Value.Length > 0)
            {
                using (var memoryStream = new MemoryStream(value.Value))
                {
                    var originalValue = Formatter.Deserialize(memoryStream);
                    return originalValue;
                }
            }
            
            return null;
        }

        public static List<InvokeArgument> Serialize(object[] values)
        {
            var list = new List<InvokeArgument>();
            for (int index = 0; index < values.Length; index++)
            {
                var value = values[index];
                
                var argument = new InvokeArgument();

                if (value != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        Formatter.Serialize(memoryStream, value);
                        argument.Value = memoryStream.ToArray();
                    }
                }
                
                list.Add(argument);
            }

            return list;
        }

        public static List<object> Deserialize(List<InvokeArgument> arguments)
        {
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
                        value = Formatter.Deserialize(memoryStream);
                    }
                }

                list.Add(value);
            }

            return list;
        }
    }
}