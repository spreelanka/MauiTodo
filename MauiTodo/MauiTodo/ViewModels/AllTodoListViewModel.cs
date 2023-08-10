using System;
using MauiTodo.Services;
using MauiTodo.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Web;

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
                TodoLists = new Dictionary<int, TodoList>{
                            {5000, new TodoList
                            {
                                Title = "defaultlist",
                                Items = new Dictionary<int, TodoItem>{
                                    {500, new TodoItem{
                                        Id =500,
                                        Title = $"empty{500}"
                                    } }
                                }
                            } }
                }
            }
        };
        //IEnumerable<TodoList> todoLists;
        //public IEnumerable<TodoList> TodoLists
        //{
        //    get =>
        //}

        public AllTodoListViewModel(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
            getData();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            getData();
            //var raw = HttpUtility.UrlDecode(query[nameof(TodoList.Id)].ToString());
            //int id;
            //if (int.TryParse(raw, out id))
            //    Task.Run(async () =>
            //    {
            //        TodoList = await dataProvider.Get<TodoList>(id);
            //        TodoList.PropertyChanged += TodoList_PropertyChanged;
            //    });
        }

        async Task getData()
        {
            var d = await dataProvider.Get<Data>(0);
            var l = d.AllTodoLists;
            Data = d;
        }


    }
}

