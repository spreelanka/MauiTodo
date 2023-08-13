using System;

namespace MauiTodo.Services
{
    public class ShellNavigation : IShellNavigation
    {
        public Task GoToAsync(ShellNavigationState state)
        {
            return Shell.Current.GoToAsync(state);
        }
    }
}

