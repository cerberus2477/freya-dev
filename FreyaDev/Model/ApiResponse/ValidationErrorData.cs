//using System;
//using System.Collections.Generic;

//namespace FreyaDev.Model
//{
//    /// Non-generic interface for accessing validation error details.
//    /// Useful for general error handling without needing the data type.
//    public interface IValidationErrorData
//    {
//        Dictionary<string, List<string>> Errors { get; set; }
//    }

//    /// Generic interface that combines the specific data type and validation error structure.
//    public interface IValidationErrorData<T> : IData, IValidationErrorData where T : IData
//    {
//    }

//    /// Generic implementation of validation error data.
//    /// Can be used for login, register, or any other IData-based response.
//    public class ValidationErrorData<T> : IValidationErrorData<T> where T : IData
//    {
//        public Dictionary<string, List<string>> Errors { get; set; } = new();
//    }
//}
