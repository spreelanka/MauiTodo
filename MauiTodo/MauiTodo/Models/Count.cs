using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiTodo.Models
{
	[ObservableObject]
	public partial class Count
	{
		[ObservableProperty]
		int value;
	}
}

