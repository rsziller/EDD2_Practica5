using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Practica5EDD2.util
{
    public class RSA
    {

        public static byte[] SignMessage(string message, BigInteger privateKey, BigInteger n)
        {
            // Calcular el resumen (hash) del mensaje usando SHA-256
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                byte[] messageHash = sha256.ComputeHash(messageBytes);

                // Convertir el hash del mensaje a un BigInteger
                BigInteger messageHashBigInt = new BigInteger(messageHash);

                // Firmar el resumen con la clave privada
                BigInteger digitalSignature = BigInteger.ModPow(messageHashBigInt, privateKey, n);

                // Convertir la firma a bytes y devolverla
                return digitalSignature.ToByteArray();
            }
        }

        public static bool VerifySignature(string message, byte[] digitalSignature, BigInteger publicKey, BigInteger n)
        {
            // Calcular el resumen (hash) del mensaje usando SHA-256
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                byte[] messageHash = sha256.ComputeHash(messageBytes);

                // Convertir el hash del mensaje a un BigInteger
                BigInteger messageHashBigInt = new BigInteger(messageHash);

                // Convertir la firma a un BigInteger
                BigInteger signature = new BigInteger(digitalSignature);

                // Verificar la firma digital con la clave pública
                BigInteger decryptedSignature = BigInteger.ModPow(signature, publicKey, n);

                // Comparar el resumen del mensaje con la firma desencriptada
                return messageHashBigInt.Equals(decryptedSignature);
            }
        }


        public static Keys GenerateKeys(int keySize)
        {
            // Generar dos números primos grandes p y q
            BigInteger p = GenerateLargePrime(keySize / 8);
            BigInteger q = GenerateLargePrime(keySize / 8);

            // Calcular n = p * q y φ(n) = (p - 1)(q - 1)
            BigInteger n = p * q;
            BigInteger phi = (p - 1) * (q - 1);

            // Elegir un exponente de encriptación e que sea coprimo con φ(n)
            BigInteger e = GenerateLargePrime(keySize / 8); // Valor típico para e
            //BigInteger e = 65537; // Valor típico para e

            // Calcular el exponente de desencriptación d
            BigInteger d = ModInverse(e, phi);

            return new Keys(e, d, n);
        }

        private static BigInteger GenerateLargePrime(int bits)
        {
            // Implementa la generación de un número primo grande
            Random random = new Random();
            BigInteger primeCandidate = new BigInteger();

            while (true)
            {
                byte[] buffer = new byte[bits / 8];
                random.NextBytes(buffer);
                primeCandidate = new BigInteger(buffer);
                if (IsProbablePrime(primeCandidate, 5))
                    return primeCandidate;
            }
        }

        private static bool IsProbablePrime(BigInteger source, int certainty)
        {
            if (source == 2 || source == 3)
                return true;

            if (source < 2 || source % 2 == 0)
                return false;

            // Escribir (source - 1) como 2^r * d
            BigInteger d = source - 1;
            int r = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                r++;
            }

            Random rand = new Random();

            for (int i = 0; i < certainty; i++)
            {
                BigInteger a = GenerateRandomBase(source, rand);
                BigInteger x = BigInteger.ModPow(a, d, source);

                if (x == 1 || x == source - 1)
                    continue;

                for (int j = 0; j < r - 1; j++)
                {
                    x = BigInteger.ModPow(x, 2, source);

                    if (x == 1)
                        return false;

                    if (x == source - 1)
                        break;
                }

                if (x != source - 1)
                    return false;
            }

            return true;
        }

        private static BigInteger GenerateRandomBase(BigInteger source, Random rand)
        {
            // Genera un número aleatorio entre 2 y source - 2
            byte[] bytes = source.ToByteArray();
            BigInteger max = new BigInteger(bytes) - 2;

            byte[] randomBytes = new byte[bytes.Length];
            rand.NextBytes(randomBytes);
            BigInteger result = new BigInteger(randomBytes);

            if (result < 2)
                result += 2;

            return result % max;
        }

        private static BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            // Implementa el cálculo del inverso modular de a en m
            BigInteger m0 = m;
            BigInteger y = 0, x = 1;

            while (a > 1)
            {
                BigInteger q = a / m;
                BigInteger mCopy = m;

                m = a % m;
                a = mCopy;
                BigInteger yCopy = y;

                y = x - q * y;
                x = yCopy;
            }

            // Asegurarse de que el resultado sea positivo
            return x < 0 ? x + m0 : x;
        }

        public static BigInteger Encrypt(string plaintext, BigInteger publicKey, BigInteger n)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(plaintext);
            BigInteger messageBigInt = new BigInteger(messageBytes);
            // Cifrado: ciphertext = plaintext^publicKey mod n
            return BigInteger.ModPow(messageBigInt, publicKey, n);
        }

        public static BigInteger Decrypt(BigInteger ciphertext, BigInteger privateKey, BigInteger n)
        {
           
            // Descifrado: plaintext = ciphertext^privateKey mod n
            return BigInteger.ModPow(ciphertext, privateKey, n);
        }
    }
}
