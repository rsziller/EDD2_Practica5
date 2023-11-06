using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Practica5EDD2.util
{
    public class Keys
    {

        public BigInteger PublicKey { get; private set; } // (e, n)
        public BigInteger PrivateKey { get; private set; } // (d, n)
        public BigInteger N { get; private set; } // Modulus

        public Keys(BigInteger publicKey, BigInteger privateKey, BigInteger modulus)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
            N = modulus;
        }
    }
}
