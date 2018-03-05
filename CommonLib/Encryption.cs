using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class Encryption
    {
        //public static BigInteger Three = new BigInteger((uint)3);
        //public static BigInteger Module = new BigInteger("f488fd584e49dbcd20b49de49107366b336c380d451d0f7c88b31c7c5b2d8ef6f3c923c043f0a55b188d8ebb558cb85d38d334fd7c175743a31d186cde33212cb52aff3ce1b1294018118d7c84a70a72d686c40319c807297aca950cd9969fabd00a509b0246d3083d66a45d419f9c7cbd894b221926baaba25ec355e92f78c7");
        //public static BigInteger Module = new BigInteger("175012832246148469004952309893923119007504294868274830650101802243580016468616226644476369579140157420542034349400995694097261371077961674039236035533383172308367706779425637041402045013194820474112524204508905916696893254410707373670063475235242589213472899328698912258375583335003993274863729669402122894589");


        //BigInteger privateKey = Three;
        //byte[] aesKey;
        Rijndael aes;
        ICryptoTransform encryptor;
        ICryptoTransform decryptor;

        public Encryption()
        {
            aes = Rijndael.Create();
            aes.Mode = CipherMode.ECB;
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.Zeros;
        }

        ~Encryption()
        {
            encryptor.Dispose();
            decryptor.Dispose();
            aes.Dispose();
        }

        public void SetAesKey(byte[] key)
        {
            //var aesKey = key;
            decryptor = aes.CreateDecryptor(key, null);
            encryptor = aes.CreateEncryptor(key, null);         
        }


        //public void MakePrivateKey()
        //{
        //    SHA1 sha = SHA1.Create();
        //    byte[] tmp = new byte[40];
        //    sha.TransformBlock(System.Text.Encoding.ASCII.GetBytes(DateTime.Now.ToString() + DateTime.Now.ToUniversalTime() + DateTime.Now.ToLongDateString()), 0, 40, tmp, 0);
        //    privateKey = new BigInteger(tmp);
        //}

        //public byte[] GetKeyExchangeBytes()
        //{
        //    if (privateKey == Three)
        //        return null;
        //    return Three.modPow(privateKey, Module).getBytes();
        //}

        //public void MakeAESKey(string keyExchangeBytes)
        //{
        //    BigInteger A = new BigInteger(keyExchangeBytes);
        //    byte[] R = A.modPow(privateKey, Module).getBytes();
        //    aesKey = new byte[16];
        //    Array.Copy(R, aesKey, 16);
        //    for (int i = 0; i < 16; i++)
        //    {
        //        byte tmp = (byte)(aesKey[i] >> 4);
        //        byte tmp2 = (byte)(aesKey[i] & 0xF);
        //        if (tmp > 9)
        //            tmp = (byte)(tmp - 9);
        //        if (tmp2 > 9)
        //            tmp2 = (byte)(tmp2 - 9);
        //        aesKey[i] = (byte)( tmp << 4 | tmp2);
        //    }
        //}

        public bool IsReady
        {
            get
            {
                return encryptor != null && decryptor != null;
            }
        }

        public byte[] Encrypt(byte[] src, int offset = 8)
        {
            if (encryptor == null) return src;
            //if (offset >= src.Length) return src;

            int len = src.Length;
            byte[] length1 = len.ToBytes(); 
            int len2 = ((len + 15) / 16) * 16;
            byte[] length2 = len2.ToBytes();
            byte[] buf = new byte[8];
            Buffer.BlockCopy(length2, 0, buf, 0, 4);
            Buffer.BlockCopy(length1, 0, buf, 4, 4);
            byte[] encrypted;
            using (MemoryStream msEncrypt = new MemoryStream())
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                csEncrypt.Write(src, 0, src.Length);
                csEncrypt.FlushFinalBlock();
                encrypted = msEncrypt.ToArray();
            }
            return buf.Concat(encrypted).ToArray();
        }

        public byte[] Decrypt(byte[] src, int offset = 8)
        {
            if (decryptor == null) return src;
            if (offset >= src.Length) return src;
            
            int len = src.Length - offset;
            //src.CopyTo(buf, 0);
            //decryptor.TransformBlock(src, offset, len, buf, 0);
            //return buf;
            using (var msDecrypt = new MemoryStream())
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
            {
                csDecrypt.Write(src.Skip(offset).ToArray(), 0, len);
                csDecrypt.FlushFinalBlock();
                return msDecrypt.ToArray();
            }
        }
    }
}
