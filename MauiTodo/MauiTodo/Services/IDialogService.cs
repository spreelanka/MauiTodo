using System;
namespace MauiTodo.Services
{
    public interface IDialogService
    {
        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);
    }
}

