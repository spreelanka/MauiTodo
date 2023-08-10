using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiTodo.Models
{
    [ObservableObject]
    public partial class Data
    {
        [ObservableProperty]
        Count count;
        [ObservableProperty]
        AllTodoLists allTodoLists;
    }
}

