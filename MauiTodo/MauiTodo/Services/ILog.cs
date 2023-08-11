using System;
namespace MauiTodo.Services
{
    public interface ILog
    {
        void Error(string message, Exception ex);
        void Warn(string message, Exception ex);
    }
}

