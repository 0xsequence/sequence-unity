using AppleAuth.Interfaces;
using System;

namespace AppleAuth.Native
{
    [Serializable]
#if UNITY_2017_1_OR_NEWER
    internal class PasswordCredential : IPasswordCredential, UnityEngine.ISerializationCallbackReceiver
#else
    internal class PasswordCredential : IPasswordCredential
#endif
    {
        public string _user = null;
        public string _password = null;
        
        public string User { get { return this._user; } }
        public string Password { get { return this._password; } }
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            SerializationTools.FixSerializationForString(ref this._user);
            SerializationTools.FixSerializationForString(ref this._password);
        }
    }
}
