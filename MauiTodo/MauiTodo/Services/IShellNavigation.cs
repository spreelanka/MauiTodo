using System;
namespace MauiTodo.Services
{
    public interface IShellNavigation
    {
        Task GoToAsync(ShellNavigationState state);
    }
}

