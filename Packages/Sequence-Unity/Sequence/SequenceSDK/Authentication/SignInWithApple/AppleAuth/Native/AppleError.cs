using AppleAuth.Interfaces;
using System;

namespace AppleAuth.Native
{
    [Serializable]
#if UNITY_2017_1_OR_NEWER
    internal class AppleError : IAppleError, UnityEngine.ISerializationCallbackReceiver
#else
    internal class AppleError : IAppleError
#endif
    {
        public int _code = 0;
        public string _domain = null;
        public string _localizedDescription = null;
        public string[] _localizedRecoveryOptions = null;
        public string _localizedRecoverySuggestion = null;
        public string _localizedFailureReason = null;
        
        public int Code { get { return this._code; } }
        public string Domain { get { return this._domain; } }
        public string LocalizedDescription { get { return this._localizedDescription; } }
        public string[] LocalizedRecoveryOptions { get { return this._localizedRecoveryOptions; } }
        public string LocalizedRecoverySuggestion { get { return this._localizedRecoverySuggestion; } }
        public string LocalizedFailureReason { get { return this._localizedFailureReason; } }
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            SerializationTools.FixSerializationForString(ref this._domain);
            SerializationTools.FixSerializationForString(ref this._localizedDescription);
            SerializationTools.FixSerializationForString(ref this._localizedRecoverySuggestion);
            SerializationTools.FixSerializationForString(ref this._localizedFailureReason);
            
            SerializationTools.FixSerializationForArray(ref this._localizedRecoveryOptions);
        }

        public override string ToString()
        {
            return $"Domain={_domain} Code={_code} Description={_localizedDescription}";
        }
    }
}