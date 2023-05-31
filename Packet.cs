using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace TextRpg_packet
{
    public enum Packet_Type
    {
        보스=0,
        챕터,
        패턴,
        에러,
        종료
        
    }
    [Serializable]
    public class Packet
    {
        public int Length;
        public int Typee;

        public Packet()
        {
            this.Length = 0;
            this.Typee = 0;
        }
        sealed class PreMergeToMergedDeserializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type typeToDeserialize = null;

                // For each assemblyName/typeName that you want to deserialize to
                // a different type, set typeToDeserialize to the desired type.
                String exeAssembly = Assembly.GetExecutingAssembly().FullName;


                // The following line of code returns the type.
                typeToDeserialize = Type.GetType(String.Format("{0}, {1}",
                    typeName, exeAssembly));

                return typeToDeserialize;
            }
        }
        public static byte[] Serialize(Object o)
        {
            MemoryStream ms = new MemoryStream(256);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, o);
            ms.Flush();
            return ms.ToArray();
        }
        public static Object Desserialize(byte[] bt)
        {
            MemoryStream ms = new MemoryStream(256);
            ms.Write(bt, 0, bt.Length);
            ms.Position = 0;
            BinaryFormatter bf = new BinaryFormatter();
            bf.Binder = new PreMergeToMergedDeserializationBinder();
            Object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;
        }
    }
    [Serializable]
    public class Boss : Packet
    {
        public int value = 0;
    }
    [Serializable]
    public class Pattern : Packet
    {
        public int chapter = 0;
        public int pattern = 0;
    }

}