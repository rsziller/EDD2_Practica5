using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Practica5EDD2.util
{
     class Password
    {

        public string Recluiter { get; set; }
        public string Company { get; set; }
        public BigInteger Contrasena { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;

            Password that = (Password)obj;
            return Recluiter.Equals(that.Recluiter) && Company.Equals(that.Company);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Recluiter, Company);
        }
    }
}
