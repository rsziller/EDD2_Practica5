using Practica5EDD2.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Practica5EDD2.util
{
     class PasswordGiver
    {
        Keys keyPair = RSA.GenerateKeys(2048);

        public HashSet<Password> ProvideCredentials(List<Candidates> aplicantes)
        {
            HashSet<Password> credenciales = new HashSet<Password>();

            foreach (var aplicante in aplicantes)
            {
                foreach (var company in aplicante.Companies)
                {
                    Password credencial = new Password
                    {
                        Recluiter = aplicante.Recluiter,
                        Company = company
                    };
                    credenciales.Add(credencial);
                }
            }



            string path = @"C:\\Users\\Rolando Ziller\\Documents\\Universidad\\2023\\segundo ciclo\\edd2\\ingresolab5";
            Files.EscribirArchivo(path, "private.rsa", Convert.ToBase64String(Encoding.UTF8.GetBytes(keyPair.PrivateKey.ToString())));
            Files.EscribirArchivo(path, "public.rsa", Convert.ToBase64String(Encoding.UTF8.GetBytes(keyPair.PublicKey.ToString())));

            foreach (Password credencial in credenciales)
            {
                string pass = credencial.Recluiter[0].ToString() + credencial.Company[0].ToString() + "2023..";
                BigInteger password = RSA.Encrypt(pass, keyPair.PrivateKey, keyPair.N);
                credencial.Contrasena = password;

            }

            return credenciales;
        }

        public bool ValidateCredentials(HashSet<Password> credenciales, string recluiter, string company, string password)
        {
            Password credendial = credenciales.FirstOrDefault(credencial => credencial.Recluiter == recluiter && credencial.Company == company);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            BigInteger passwordBigInt = new BigInteger(passwordBytes);
            if (credendial != null)
            {
                BigInteger passwordDecoded = RSA.Decrypt(credendial.Contrasena, keyPair.PublicKey, keyPair.N);
                return passwordDecoded.Equals(passwordBigInt);
            }

            return false;
        }

    }
}
