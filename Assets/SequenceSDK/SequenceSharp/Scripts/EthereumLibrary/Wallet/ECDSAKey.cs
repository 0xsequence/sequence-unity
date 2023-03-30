using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Linq;
using System.Text;

namespace SequenceSharp.WALLET
{

    public class ECDSAKey
    {
        public static readonly ECDomainParameters domainParams;
        internal static readonly X9ECParameters secp256k1;
        private static readonly AsymmetricCipherKeyPair keyPair;
        public ECPrivateKeyParameters PrivateKey => keyPair.Private  as ECPrivateKeyParameters;
        public ECPublicKeyParameters PublicKey => keyPair.Public as ECPublicKeyParameters;
        static ECDSAKey()
        {
            
            secp256k1 = ECNamedCurveTable.GetByName("secp256k1");
            domainParams = new ECDomainParameters(secp256k1.Curve, secp256k1.G, secp256k1.N, secp256k1.H, secp256k1.GetSeed());
            var secureRandom = new SecureRandom();
            var keyParams = new ECKeyGenerationParameters(domainParams, secureRandom);

            var generator = new ECKeyPairGenerator("ECDSA");
            generator.Init(keyParams);
            keyPair = generator.GenerateKeyPair();
        }




        

    }

}
