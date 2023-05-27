﻿namespace rest_api.Models
{
    public class ResponseModel<T>
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}