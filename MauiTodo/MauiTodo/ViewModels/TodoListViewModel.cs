using System;
using System.Web;
using MauiTodo.Models;
using MauiTodo.Services;

namespace MauiTodo.ViewModels
{
    public class TodoListViewModel : BaseObservable, IQueryAttributable
	{
        TodoList todoList;
        public TodoList TodoList{
            get => todoList;
            set
            {
                todoList = value;
                OnPropertyChanged();
            }
        }

        IDataProvider dataProvider;
        public TodoListViewModel(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var raw = HttpUtility.UrlDecode(query[nameof(TodoList.Id)].ToString());
            int id;
            if (int.TryParse(raw, out id))
                Task.Run(async () => TodoList = await dataProvider.Get<TodoList>(id));
        }
    }
}

