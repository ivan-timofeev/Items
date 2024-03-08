using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Items.Models.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string message, Dictionary<string, string> additionalInformaton)
            : base(message)
        {
            foreach (var element in additionalInformaton)
            {
                Data.Add(element.Key, element.Value);
            }
        }

        public BusinessException(string message)
            : base(message)
        {

        }
    }
}
