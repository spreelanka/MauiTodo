using System;
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
		Dictionary<int, TodoItem> items;
	}
}

