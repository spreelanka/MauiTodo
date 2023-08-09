using MauiTodo.ViewModels;

namespace MauiTodo.Views;

public partial class TodoListPage : ContentPage
{

	public TodoListPage(TodoListViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}
}
