using System;
using System.Text;
using UnityEngine;
using System.IO;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using ProtectedDataWrapper;
#endif

namespace Sequence.Utils.SecureStorage
{
    public class WindowsProtectedDataStorage : ISecureStorage
    {
        public static string DataFile { get { return $"{Application.persistentDataPath}/data.bin"; } }

        public WindowsProtectedDataStorage()
        {
#if !UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN
            throw new System.NotSupportedException("WindowsProtectedDataStorage is only supported on windows platform.");
#endif
        }

        public void StoreString(string key, string value)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            byte[] data = Util.EncryptData(value, Encoding.UTF8.GetBytes(key));

            using (FileStream fs = new FileStream(DataFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(data);
                }
            }
#endif
        }

        public string RetrieveString(string key)
        {
            string value = "";

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

            byte[] data = null;
            using (FileStream fs = new FileStream(DataFile, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    data = br.ReadBytes((int)br.BaseStream.Length);
                }
            }

            value = Util.DecryptData(data, Encoding.UTF8.GetBytes(key));
#endif

            return value;
        }
    }
}
