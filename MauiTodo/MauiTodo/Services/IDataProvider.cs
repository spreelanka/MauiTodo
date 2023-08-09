using System;
namespace MauiTodo.Services
{
	public interface IDataProvider
	{
        Task<T> Get<T>(int id);
        Task Put<T>(T data);
	}
}

