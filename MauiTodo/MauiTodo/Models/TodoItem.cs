using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiTodo.Models
{
	[ObservableObject]
	public partial class TodoItem
	{
		[ObservableProperty]
		int id;
		[ObservableProperty]
		bool complete;
        [ObservableProperty]
        string title;
	}
	
}

