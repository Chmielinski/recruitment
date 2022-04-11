using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Models.Exception
{
    public class BusinessLogicException : System.Exception
    {
        public BusinessLogicException() : base() { }

        public BusinessLogicException(string message) : base(message) { }

        public BusinessLogicException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
