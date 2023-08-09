using MauiTodo.ViewModels;

namespace MauiTodo.Views;

public partial class AllTodoListsPage : ContentPage
{
	public AllTodoListsPage(AllTodoListViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
