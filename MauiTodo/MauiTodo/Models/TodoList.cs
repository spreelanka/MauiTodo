using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiTodo.Models
{
    [ObservableObject]
    public partial class TodoList
    {
        [ObservableProperty]
        int id;
        [ObservableProperty]
        string title;
        [ObservableProperty]
        ObservableCollection<TodoItem> items = new ObservableCollection<TodoItem>();
    }
}

