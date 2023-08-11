using System;
using System.Web;
using MauiTodo.Models;
using MauiTodo.Services;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiTodo.ViewModels
{
    [ObservableObject]
    public partial class TodoListViewModel : IQueryAttributable
    {
        [ObservableProperty]
        TodoList todoList;

        ILog log;
        IDataProvider dataProvider;
        IShellNavigation navigation;

        public TodoListViewModel(IDataProvider dataProvider, ILog log, IShellNavigation navigation)
        {
            this.log = log;
            this.dataProvider = dataProvider;
            this.navigation = navigation;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var idKey = nameof(TodoList.Id);
            if (query != null && query.ContainsKey(idKey))
            {
                var raw = HttpUtility.UrlDecode(query[idKey].ToString());
                int id;
                if (int.TryParse(raw, out id))
                    Task.Run(async () =>
                    {
                        TodoList = await dataProvider.Get<TodoList>(id);
                    });
            }
        }

        [RelayCommand]
        async Task DeleteTodoList()
        {
            await navigation.GoToAsync($"..?Delete={TodoList.Id}");
        }

        [RelayCommand]
        async Task DeleteTodoItem(int id)
        {
            TodoList.Items.Remove(
                TodoList.Items.Where(e => e.Id == id).FirstOrDefault()
            );
            await dataProvider.Put(TodoList);
        }

        [RelayCommand]
        async Task GoBack()
        {
            await dataProvider.Put(TodoList);

            await navigation.GoToAsync("..?q=1");
        }

        [RelayCommand]
        async Task AddTodoItem()
        {
            var newId = TodoList.Items.Count == 0 ? 1 : TodoList.Items.Max(e => e.Id) + 1;
            TodoList.Items.Insert(0, new TodoItem { Id = newId });
            await dataProvider.Put(TodoList);
            await dataProvider.Save();
        }
    }
}

