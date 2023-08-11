using System;
using System.Web;
using MauiTodo.Models;
using MauiTodo.Services;
//using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiTodo.ViewModels
{
    [ObservableObject]
    public partial class TodoListViewModel : IQueryAttributable
    {
        [ObservableProperty]
        TodoList todoList;


        IDataProvider dataProvider;
        public TodoListViewModel(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
            this.PropertyChanged += TodoListViewModel_PropertyChanged;
        }

        private void TodoListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //dataProvider.Put(TodoList);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var raw = HttpUtility.UrlDecode(query[nameof(TodoList.Id)].ToString());
            int id;
            if (int.TryParse(raw, out id))
                Task.Run(async () =>
                {
                    TodoList = await dataProvider.Get<TodoList>(id);
                    TodoList.PropertyChanged += TodoList_PropertyChanged;
                });
        }

        private void TodoList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
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

            await Shell.Current.GoToAsync("..?qqq=1");
        }

        [RelayCommand]
        async Task AddTodoItem()
        {
            TodoList.Items.Insert(0, new TodoItem { Title = "new item", Id = TodoList.Items.Max(e => e.Id) + 1 });
            await dataProvider.Put(TodoList);
            await dataProvider.Save();
        }
    }
}

