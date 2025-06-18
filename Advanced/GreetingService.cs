using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advanced
{
    public class GreetingService : IGreetingService
    {
        public string GetGreeting(string name)
        {
            return $"Hello, {name}! Welcome to our .NET 9 API.";
        }
    }
}
