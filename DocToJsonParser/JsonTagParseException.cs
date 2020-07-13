using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocToJsonParser
{
    public class JsonTagParseException : Exception
    {
        public JsonTagParseException(int lineNumber, string message) : base($"{lineNumber}: {message}") { }
    }
}
