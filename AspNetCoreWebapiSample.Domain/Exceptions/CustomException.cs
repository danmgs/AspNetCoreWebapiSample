using System;

namespace AspNetCoreWebapiSample.Domain.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException()
        {
        }

        public CustomException(string message) 
            : base(message)
        {
        }

        public CustomException(string message,Exception innerException)
         : base(message,innerException)
        {
        }
    }


    public class CustomNotFoundException : CustomException
    {
        public CustomNotFoundException(string nameObject)
            :base ($"{nameObject} could not be found")
        {
        }
    }

    public class CustomFieldAlreadyExistsException : CustomException
    {
        public CustomFieldAlreadyExistsException(string field)
            :base ($"This {field} already exists")
        {
        }
    }



}
