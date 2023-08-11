using MauiTodo.Models;
using MauiTodo.Services;
using MauiTodo.ViewModels;

namespace MauiTodo.Views;

public partial class AllTodoListsPage : ContentPage
{
    ILog log;
    IShellNavigation navigation;
    public AllTodoListsPage(AllTodoListViewModel vm, ILog log, IShellNavigation navigation)
    {
        InitializeComponent();
        BindingContext = vm;
        this.log = log;
        this.navigation = navigation;
    }

    void ListView_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;
        if (e.SelectedItem is TodoList list)
        {
            (sender as ListView).SelectedItem = null;
            navigation.GoToAsync($"/{nameof(TodoListPage)}?{nameof(TodoList.Id)}={list.Id}");
            return;
        }
        log.Error($"{e.SelectedItem.GetType().Name} not supported", null);
    }
}
