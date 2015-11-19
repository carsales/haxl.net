using System;

namespace Haxl.Net.Errors
{
    public class HaxlPredicateFailedException : Exception
    {
        public HaxlPredicateFailedException(string message) 
            : base(message) { }
    }
}