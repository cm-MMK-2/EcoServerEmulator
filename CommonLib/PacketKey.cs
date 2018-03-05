using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace CommonLib
{


    /*
        DWORD unknown; // 0x00000000

        //g "3"固定
        DWORD generator_size; // 0x01
        BYTE generator[generator_size]; // 0x32

        //p (素数)
        DWORD prime_size; //0x100
        BYTE prime[prime_size];

        //公開値
        DWORD public_key_size; //0x100 桁が小さい場合、0x100以下な事も
        BYTE public_key[public_key_size]; 
     */
    public class ServerKey
    {
        public string Generator { get; private set; } = "3";

        public BigInteger Prime = BigInteger.Parse("175012832246148469004952309893923119007504294868274830650101802243580016468616226644476369579140157420542034349400995694097261371077961674039236035533383172308367706779425637041402045013194820474112524204508905916696893254410707373670063475235242589213472899328698912258375583335003993274863729669402122894589");
        

        //server private key
        public BigInteger PrivateKey { get; private set;}
        //server public key
        public BigInteger PublicKey { get; private set; }

        public byte[] PublicKeyBytes { get; private set; }

        public ServerKey()
        {
            byte[] secretkey = new Byte[64];
            //RNGCryptoServiceProvider is an implementation of a random number generator.
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                // The array is now filled with cryptographically strong random bytes.
                rng.GetBytes(secretkey);
                secretkey[63] = 0;
                PrivateKey = new BigInteger(secretkey);                
            }
            Logger.Debug($"server private key:{PrivateKey.ToString("x")}");

            PublicKey = BigInteger.ModPow(BigInteger.Parse(Generator), PrivateKey, Prime);

            Logger.Debug($"server public key:{PublicKey.ToString("x")}");
            GetBytes();
        }

        private void GetBytes()
        {
            byte[] bytes = new byte[1024];
            int offset = 4;
            //generator
            var generator_bytes = Encoding.ASCII.GetBytes(Generator);
            bytes.Fill(generator_bytes.Length.ToBytes(), 4);
            offset += 4;
            bytes.Fill(generator_bytes, offset);
            offset += generator_bytes.Length;
            //prime
            string prime_str = Prime.ToString("x").TrimStart('0').PadLeft(256, '0');
            Logger.Debug($"prime:{prime_str}, length:{prime_str.Length}");
            byte[] prime_bytes = Encoding.ASCII.GetBytes(prime_str);
            bytes.Fill(prime_bytes.Length.ToBytes(), offset);
            offset += 4;
            bytes.Fill(prime_bytes, offset);
            offset += prime_bytes.Length;
            //public key
            string public_key_str = PublicKey.ToString("x").TrimStart('0').PadLeft(256, '0');
            Logger.Debug($"public_key:{public_key_str}, length:{public_key_str.Length}");
            byte[] public_key_bytes = Encoding.ASCII.GetBytes(public_key_str);
            bytes.Fill(public_key_bytes.Length.ToBytes(), offset);
            offset += 4;
            bytes.Fill(public_key_bytes, offset);
            offset += public_key_bytes.Length;
            PublicKeyBytes = bytes.Take(offset).ToArray();
        }
    }

    public class ClientKey
    {
        public int PublicKeySize { get; set; }
        public BigInteger PublicKey { get; set; }

        public ClientKey(byte[] data)
        {
            PublicKeySize = (data[0] << 24) + (data[1] << 16) + (data[2] << 8) + data[3];
            string clientkey_str = Encoding.ASCII.GetString(data.Skip(4).ToArray()).ToLower();
            if (clientkey_str.StartsWith("0x") || clientkey_str.StartsWith("&h"))
            {
                clientkey_str = clientkey_str.Remove(0, 2);
            }
            
            PublicKey = BigInteger.Parse("0" + clientkey_str, NumberStyles.AllowHexSpecifier);
            //if(PublicKey.Sign < 0)
            //{
            //    PublicKey = new BigInteger(PublicKey.ToByteArray().Concat(new byte[] { 0 }).ToArray());
            //}

        }
    }


    public class PacketKey
    {
        public ServerKey serverKey;
        public ClientKey clientKey;

        public string GetSharedKey()
        {
            //pow(client_public_key, server_private_key, prime)
            // clientKey.PublicKey, serverKey.PrivateKey, serverKey.prime
            //Logger.Debug($"clientKey.PublicKey:{clientKey.PublicKey.ToString("x")}\nserverKey.PrivateKey:{serverKey.PrivateKey.ToString("x")}\nserverKey.Prime:{serverKey.Prime.ToString("x")}");
            BigInteger sharedKeyInt = BigInteger.ModPow(clientKey.PublicKey, serverKey.PrivateKey, serverKey.Prime);
            //byte[] sharedKeyBytes = sharedKeyInt.ToByteArray().Reverse().ToArray();
            string SharedKey = sharedKeyInt.ToString("x").TrimStart('0').PadLeft(256, '0');

            return SharedKey;
        }

        public byte[] GetRijndaelKey(byte[] sharedKey)
        {
            byte[] key_bytes = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                if (sharedKey[i] >= 'a')
                {
                    key_bytes[i] = (byte)(sharedKey[i] - 48);
                }
                else
                {
                    key_bytes[i] = sharedKey[i];
                }
            }

            string key_str = Encoding.ASCII.GetString(key_bytes);
            Logger.Info($"rijndael key:{key_str}");
            return key_str.ToHexByteArray();
        }
    }
}
