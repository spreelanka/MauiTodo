using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiTodo.Models
{
    [ObservableObject]
    public partial class AllTodoLists
    {
        [ObservableProperty]
        ObservableCollection<TodoList> todoLists = new ObservableCollection<TodoList>();
    }
}

