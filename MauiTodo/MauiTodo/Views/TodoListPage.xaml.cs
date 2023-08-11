using MauiTodo.Models;
using MauiTodo.ViewModels;

namespace MauiTodo.Views;

public partial class TodoListPage : ContentPage
{

    public TodoListPage(TodoListViewModel vm)
    {
        InitializeComponent();
        this.BindingContext = vm;
    }

    void ListView_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectedItemChangedEventArgs e)
    {
        // option to have a detail page here
    }
}
