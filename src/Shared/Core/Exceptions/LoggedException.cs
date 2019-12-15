using System;
using System.Collections.Generic;

namespace Cinder.Core.Exceptions
{
    public class LoggedException : AggregateException
    {
        public LoggedException() { }
        public LoggedException(IEnumerable<Exception> innerExceptions) : base(innerExceptions) { }
        public LoggedException(params Exception[] innerExceptions) : base(innerExceptions) { }
        public LoggedException(string message) : base(message) { }
        public LoggedException(string message, IEnumerable<Exception> innerExceptions) : base(message, innerExceptions) { }
        public LoggedException(string message, Exception innerException) : base(message, innerException) { }
        public LoggedException(string message, params Exception[] innerExceptions) : base(message, innerExceptions) { }
    }
}
