using System;
using MauiTodo.Services;

namespace MauiTodo.ViewModels
{
	public class TodoListViewModel : BaseObservable
	{
        IDataProvider dataProvider;
        public TodoListViewModel(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }
    }
}

