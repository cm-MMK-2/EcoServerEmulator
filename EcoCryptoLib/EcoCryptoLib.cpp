// EcoCryptoLib.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "EcoCryptoLib.h"


const CryptoPP::Integer Prime = CryptoPP::Integer("175012832246148469004952309893923119007504294868274830650101802243580016468616226644476369579140157420542034349400995694097261371077961674039236035533383172308367706779425637041402045013194820474112524204508905916696893254410707373670063475235242589213472899328698912258375583335003993274863729669402122894589");
const char * Generator = "3";
const CryptoPP::Integer Prime;
CryptoPP::Integer SPrivateKey;
CryptoPP::Integer ServerPublicKey;
BYTE RijndaelKey[32];


void GenerateServerPublicKey()
{
	CryptoPP::AutoSeededRandomPool rng;
	SPrivateKey = CryptoPP::Integer(rng, 512);
	ServerPublicKey = CryptoPP::a_exp_b_mod_c(CryptoPP::Integer(Generator), SPrivateKey, Prime);
}

void GetServerPublicKeyBytes(BYTE* buf, size_t& offset)
{
	offset = 4;
	//generator
	size_t generator_length = strlen(Generator);
	memcpy(buf + offset, &generator_length, 4);
	offset += 4;
	memcpy(buf + offset, Generator, generator_length);
	offset += generator_length;

	//prime
	std::ostringstream prime_ss;
	prime_ss << std::hex << Prime;
	std::string prime_str = prime_ss.str();
	size_t prime_size = prime_str.size() - 1;
	memcpy(buf + offset, &prime_size, 4);
	offset += 4;
	memcpy(buf + offset, prime_str.c_str(), prime_size);
	offset += prime_size;

	std::cout << "prime:" << prime_ss.str() << std::endl;

	//public key
	std::ostringstream spublic_ss;
	//ServerPublicKey.Encode();
	spublic_ss << std::hex << ServerPublicKey;
	std::string spublic_str = spublic_ss.str();
	size_t spublickey_size = spublic_str.size() - 1;
	memcpy(buf + offset, &spublickey_size, 4);
	offset += 4;
	memcpy(buf + offset, spublic_str.c_str(), spublickey_size);
	offset += spublickey_size;

	std::cout << "Server Public Key:" << spublic_ss.str() << std::endl;
}

//data - client public key 
BYTE* CalculateRijndaelKey(BYTE* data)
{
	int cpKeySize = (data[0] << 24) + (data[1] << 16) + (data[2] << 8) + data[3];
	BYTE* buf = new BYTE[cpKeySize + 1];
	buf[cpKeySize] = 0;
	memcpy(buf, data, cpKeySize);
	CryptoPP::Integer clientPublicKey = CryptoPP::Integer((char *)buf);
	delete buf;
	std::cout << "clientPublicKey:" << std::hex << clientPublicKey << std::endl;

	CryptoPP::Integer sharedKey = CryptoPP::a_exp_b_mod_c(clientPublicKey, SPrivateKey, Prime);

	//Prime = NULL;
	SPrivateKey = NULL;
	ServerPublicKey = NULL;
	std::cout << "sharedKey:" << std::hex << sharedKey << std::endl;
	std::string key_sink;
	ServerPublicKey.DEREncodeAsOctetString(CryptoPP::StringSink(key_sink), 32);
	const char* key_buf = key_sink.c_str();
	for (int i = 0; i < 32; i++)
	{
		if (buf[i] >= 'a')
		{
			RijndaelKey[i] = (BYTE)(key_buf[i] - 48);
		}
		else
		{
			RijndaelKey[i] = key_buf[i];
		}
	}
	return RijndaelKey;
}