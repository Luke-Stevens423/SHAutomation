﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SHAutomation.Core.Exceptions
{
    [Serializable]
    public class NoClickablePointException : SHAutomationException
    {
        public NoClickablePointException()
        {
        }

        public NoClickablePointException(string message)
            : base(message)
        {
        }

        public NoClickablePointException(Exception innerException)
            : base(string.Empty, innerException)
        {
        }

        public NoClickablePointException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected NoClickablePointException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
