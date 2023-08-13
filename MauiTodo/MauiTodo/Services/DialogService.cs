using System;

namespace MauiTodo.Services
{
    public class DialogService : IDialogService
    {
        public Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            var page = Application.Current?.MainPage ?? throw new NullReferenceException();
            return page.DisplayAlert(title, message, accept, cancel);
        }
    }
}

