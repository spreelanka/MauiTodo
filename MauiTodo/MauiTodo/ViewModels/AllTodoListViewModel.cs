using System;
using MauiTodo.Services;
using MauiTodo.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Web;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiTodo.ViewModels
{
    [ObservableObject]
    public partial class AllTodoListViewModel : IQueryAttributable
    {
        IDataProvider dataProvider;
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
                                Title = "defaultlist",
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

        public AllTodoListViewModel(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
            getData();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            getData();
            if (query.ContainsKey("Delete"))
            {
                var deleteIdString = HttpUtility.UrlDecode(query["Delete"].ToString());
                if (deleteIdString != null)
                {
                    int deleteId = 0;
                    if (int.TryParse(deleteIdString, out deleteId))
                    {
                        Data.AllTodoLists.TodoLists.Remove(
                            Data.AllTodoLists.TodoLists.Where(e => e.Id == deleteId).FirstOrDefault()
                        );
                        await dataProvider.Put(Data);
                        await dataProvider.Save();
                    }
                }
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


    }
}

