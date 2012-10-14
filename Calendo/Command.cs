using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calendo
{
    public class Command
    {
        private string _type;
        private string _parameter;
        public Command(string type, string parameter)
        {
            _type = type;
            _parameter = parameter;
        }
        public string Type
        {
            get { return _type; }
        }
        public string Parameter
        {
            get { return _parameter; }
        }
    }
}
