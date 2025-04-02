using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Utils.Shared
{
    public class GenericResponse<T>
    {
        public T? Data { get; set; } 
        public string? Message { get; set; }
        public bool Success { get; set; } = false;

        public GenericResponse<T> WithData(T data)
        {
            Data = data;
            return this;
        }

        public GenericResponse<T> WithMessage(string? message)
        {
            Message = message;
            return this;
        }

        public GenericResponse<T> WithSuccess()
        {
            Success = true;
            return this;
        }

        public GenericResponse<T> WithFail()
        {
            Success = false;
            return this;
        }

    }

    public class GenericResponse 
    {
        
        public string? Message { get; set; }
        public bool Success { get; set; } = false;
 

        public GenericResponse  WithMessage(string? message)
        {
            Message = message;
            return this;
        }

        public GenericResponse  WithSuccess()
        {
            Success = true;
            return this;
        }

        public GenericResponse  WithFail()
        {
            Success = false;
            return this;
        }

    }

}
