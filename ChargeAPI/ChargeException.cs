using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnerFence.ChargeAPI
{
    public class ChargeException : Exception
    {
        public ChargeException(string message)
            : base(message)
        {
        }
    }
}
