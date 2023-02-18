using System;
using System.Collections.Generic;
using System.Text;

namespace SharedProject_Azienda
{
    class Customer<T> : Persona<T> where T : struct
    {
        public override T GetEconomicValue()
        {
            throw new NotImplementedException();
        }
    }
}
