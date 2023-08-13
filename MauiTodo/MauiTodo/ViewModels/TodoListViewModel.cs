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
        IDialogService dialogService;

        public TodoListViewModel(IDataProvider dataProvider, ILog log, IShellNavigation navigation, IDialogService dialogService)
        {
            this.log = log;
            this.dataProvider = dataProvider;
            this.navigation = navigation;
            this.dialogService = dialogService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var idKey = nameof(TodoList.Id);
            object idValue;
            if (query != null && query.ContainsKey(idKey) && query.TryGetValue(idKey, out idValue) && idValue != null)
            {
                var raw = HttpUtility.UrlDecode(idValue.ToString());
                int id;
                if (int.TryParse(raw, out id))
                {
                    Task.Run(async () =>
                    {
                        TodoList = await dataProvider.Get<TodoList>(id);
                    });
                    return;
                }
            }
            throw new ArgumentException("query must contain an Id");
        }

        [RelayCommand]
        async Task DeleteTodoList()
        {
            if (await dialogService.DisplayAlert($"Delete TodoList {TodoList.Title}", "", "Yes", "No"))
            {
                await navigation.GoToAsync($"..?Delete={TodoList.Id}");
            }
        }

        [RelayCommand]
        async Task DeleteTodoItem(int id)
        {
            if (await dialogService.DisplayAlert("Delete Item", "", "Yes", "No"))
            {
                TodoList.Items.Remove(
                    TodoList.Items.Where(e => e.Id == id).FirstOrDefault()
                );
                await dataProvider.Put(TodoList);
                await dataProvider.Save();
            }
        }

        [RelayCommand]
        async Task GoBack()
        {
            await dataProvider.Put(TodoList);
            await dataProvider.Save();
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

