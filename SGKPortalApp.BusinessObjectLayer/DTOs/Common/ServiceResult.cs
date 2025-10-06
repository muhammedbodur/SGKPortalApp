﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Common
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ServiceResult<T> Ok(T data, string message = "") =>
            new() { Success = true, Data = data, Message = message };

        public static ServiceResult<T> Fail(string message) =>
            new() { Success = false, Message = message };
    }
}
