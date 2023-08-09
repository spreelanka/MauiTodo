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
        //if (e.SelectedItem == null)
        //    return;
        //if (e.SelectedItem is TodoItem item)
        //{
        //    Shell.Current.GoToAsync($"//TodoItemPage?{nameof(TodoItem.Id)}={item.Id}");//,new Dictionary<string, object> { {nameof(TodoList),item } });
        //}
    }
}
