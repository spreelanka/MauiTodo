using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiTodo.Models
{
	[ObservableObject]
	public partial class AllTodoLists
	{
		[ObservableProperty]
		Dictionary<int, TodoList> todoLists;
	}
}

