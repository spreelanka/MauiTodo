﻿using MauiTodo.Models;
using MauiTodo.ViewModels;

namespace MauiTodo.Views;

public partial class AllTodoListsPage : ContentPage
{
    public AllTodoListsPage(AllTodoListViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    void ListView_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;
        if (e.SelectedItem is TodoList list)
        {
            (sender as ListView).SelectedItem = null;
            Shell.Current.GoToAsync($"/{nameof(TodoListPage)}?{nameof(TodoList.Id)}={list.Id}");
        }
    }
}
