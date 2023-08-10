using MauiTodo.Views;

namespace MauiTodo;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute($"{nameof(AllTodoListsPage)}/{nameof(TodoListPage)}", typeof(TodoListPage));
	}
}

