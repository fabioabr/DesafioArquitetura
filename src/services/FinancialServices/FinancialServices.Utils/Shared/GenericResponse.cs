using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Utils.Shared
{
    public class GenericResponse<T>
    {
        public T? Data { get; private set; } 
        public string? Message { get; private set; }
        public bool Success { get; private set; } = false;
        public Exception? Exception { get; private set; }        
        public object[] Args { get; private set; }

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
        public GenericResponse<T> WithMessage(string? message, object[] args)
        {
            Message = message;
            Args = args;
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
        public GenericResponse<T> WithException(Exception ex)
        {
            Exception = ex;
            return this;
        }

    }

    public class GenericResponse 
    {
        
        public string? Message { get; private set; }
        public bool Success { get; private set; } = false;
        public Exception? Exception { get; private set; }
        public object[] Args { get; private set; } = [];

        public GenericResponse WithMessage(string? message, params object[] args)
        {
            Message = message;
            Args = args;
            return this;
        }
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

        public GenericResponse WithException(Exception ex)
        {
            Exception = ex;
            return this;
        }

    }

}
