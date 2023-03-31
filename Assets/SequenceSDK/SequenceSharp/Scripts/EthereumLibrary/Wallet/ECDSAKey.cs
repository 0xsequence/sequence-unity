using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SequenceSharp.ABI;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System;
using System.Linq;
using System.Text;

namespace SequenceSharp.WALLET
{
    //TODO: Will change this structure, 
    public class ECDSAKey
    {
        public static readonly ECDomainParameters domainParams;
        internal static readonly X9ECParameters secp256k1;
        private static AsymmetricCipherKeyPair keyPair; //TODO: Readonly?
        public static ECPrivateKeyParameters PrivateKey => keyPair.Private  as ECPrivateKeyParameters;
        public static ECPublicKeyParameters PublicKey => keyPair.Public as ECPublicKeyParameters;
        static ECDSAKey()
        {
            
            secp256k1 = ECNamedCurveTable.GetByName("secp256k1");
            domainParams = new ECDomainParameters(secp256k1.Curve, secp256k1.G, secp256k1.N, secp256k1.H, secp256k1.GetSeed());

            //ECDSAKeyFromRandom();

        }

        public static void ECDSAKeyFromRandom()
        {
            var secureRandom = new SecureRandom();
            var keyParams = new ECKeyGenerationParameters(domainParams, secureRandom);
            var generator = new ECKeyPairGenerator("ECDSA");
            generator.Init(keyParams);
            keyPair = generator.GenerateKeyPair();
            
        }

        public static void ECDSAKeyFromSeed(string seed)
        {
            throw new NotImplementedException();
        }

        public static string PublicKeyFromPrivateKey(string privatekey)
        {
            byte[] privateKeyBytes = SequenceCoder.HexStringToByteArray(privatekey);
            BigInteger d = new BigInteger(privateKeyBytes);

            var privateParams = new ECPrivateKeyParameters(d, domainParams);
            ECPoint q = domainParams.G.Multiply(d);
            var publicParams = new ECPublicKeyParameters(q, domainParams);

            keyPair = new AsymmetricCipherKeyPair(publicParams, privateParams);

            byte[] publicKeyBytes = publicParams.Q.GetEncoded();

            return SequenceCoder.ByteArrayToHexString(publicKeyBytes);
        }







    }

}
