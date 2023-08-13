using System;
using MauiTodo.Services;
using MauiTodo.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Web;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using MauiTodo.Views;

namespace MauiTodo.ViewModels
{
    [ObservableObject]
    public partial class AllTodoListViewModel : IQueryAttributable
    {
        IDataProvider dataProvider;
        ILog log;
        IShellNavigation navigation;

        [ObservableProperty]
        Data data = new Data
        {
            Count = new Count { Value = 500 },
            AllTodoLists = new AllTodoLists
            {
                TodoLists = new ObservableCollection<TodoList>{
                            new TodoList
                            {
                                Id=5000,
                                Title = "",
                                Items = new ObservableCollection<TodoItem>{
                                    new TodoItem{
                                        Id =500,
                                        Title = $"empty{500}"
                                    }
                                }
                            }
                }
            }
        };

        public AllTodoListViewModel(IDataProvider dataProvider, ILog log, IShellNavigation navigation)
        {
            this.log = log;
            this.navigation = navigation;
            this.dataProvider = dataProvider;
            getData();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var deleteKey = "Delete";
            if (query != null && query.ContainsKey(deleteKey))
            {
                object deleteValue;
                if (query[deleteKey] != null && query.TryGetValue(deleteKey, out deleteValue))
                {
                    var deleteIdString = HttpUtility.UrlDecode(deleteValue.ToString());
                    if (deleteIdString != null)
                    {
                        int deleteId = 0;
                        if (int.TryParse(deleteIdString, out deleteId))
                        {
                            Task.Run(async () =>
                            {
                                await getData();
                                var listToDelete = Data.AllTodoLists.TodoLists.Where(e => e.Id == deleteId).FirstOrDefault();
                                if (listToDelete != null)
                                {
                                    Data.AllTodoLists.TodoLists.Remove(listToDelete);
                                    await dataProvider.Put(Data);
                                    await dataProvider.Save();
                                }
                            });
                            return;
                        }
                    }
                }
                throw new ArgumentException("query parameter Delete value invalid");
            }
        }

        async Task getData()
        {
            var d = await dataProvider.Get<Data>(0);
            var l = d.AllTodoLists;
            Data = d;
        }

        [RelayCommand]
        async Task AddTodoList()
        {
            await dataProvider.Put(new TodoList { Title = "new todolist" });
            await dataProvider.Save();
            await getData();
        }

        public void ListView_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            if (e.SelectedItem is TodoList list)
            {
                if (sender != null && sender is ListView listview)
                    listview.SelectedItem = null;
                navigation.GoToAsync($"/{nameof(TodoListPage)}?{nameof(TodoList.Id)}={list.Id}");
                return;
            }
            log.Error($"{e.SelectedItem.GetType().Name} not supported", null);
        }
    }
}

