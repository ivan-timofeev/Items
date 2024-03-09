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
        public BusinessError BusinessError { get; private set; }

        public BusinessException(BusinessError error, Dictionary<string, string> additionalInformaton)
            : base(error.SystemName)
        {
            BusinessError = error;

            foreach (var element in additionalInformaton)
            {
                Data.Add(element.Key, element.Value);
            }
        }

        public BusinessException(BusinessError error)
            : base(error.SystemName)
        {

            BusinessError = error;
        }
    }

    public class BusinessError
    {
        public required int ErrorCode { get; set; }
        public required string SystemName { get; set; }
        public required string DisplayName { get; set; }
    }

    public static class ListOfBusinessErrors
    {
        public static readonly BusinessError ProductNotFound = new()
        {
            ErrorCode = 1,
            SystemName = "Product not found.",
            DisplayName = "Товар или услуга не найдены."
        };

        public static readonly BusinessError ProductsNotEnoughInStock = new()
        {
            ErrorCode = 2,
            SystemName = "Not enough in stock",
            DisplayName = "На складе недостаточное количество товара."
        };

        public static readonly BusinessError UserAlreadyExists = new()
        {
            ErrorCode = 3,
            SystemName = "User with provided email already exists.",
            DisplayName = "Пользователь с указанным адресом электронной почты уже существует."
        };

        public static readonly BusinessError UserNotFound = new()
        {
            ErrorCode = 4,
            SystemName = "User not found.",
            DisplayName = "Пользователь не найден."
        };
    }
}
