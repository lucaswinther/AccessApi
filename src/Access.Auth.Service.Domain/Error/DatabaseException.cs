using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Access.Auth.Service.Domain.Error
{
    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base(message) { }
    }
}
