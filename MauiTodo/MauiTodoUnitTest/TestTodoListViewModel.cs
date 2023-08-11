using System;
using System.Collections.ObjectModel;
using MauiTodo.Models;
using MauiTodo.Services;
using MauiTodo.ViewModels;
using Moq;

namespace MauiTodoUnitTest
{
    public class TestTodoListViewModel
    {
        TodoListViewModel subject;
        Mock<ILog> logMoq;
        Mock<IDataProvider> dataProviderMoq;
        Mock<IShellNavigation> navigationMoq;


        Task<TodoList> Get_MockResponse(int id, int size = 1)
        {
            var response = new TodoList
            {
                Id = id,
                Title = "test list",
                Items = new ObservableCollection<TodoItem>()
            };
            for (var i = 0; i <= size; i++)
            {
                response.Items.Add(new TodoItem
                {
                    Id = i + 1,
                    Title = $"item{i + 1}"
                });
            }

            return Task.FromResult(response);
        }
        [SetUp]
        public void Setup()
        {
            logMoq = new Mock<ILog>();
            dataProviderMoq = new Mock<IDataProvider>();
            navigationMoq = new Mock<IShellNavigation>();
            dataProviderMoq.Setup(c => c.Get<TodoList>(It.IsAny<int>()))
                .Returns((int id) => Get_MockResponse(id, 3))
                .Verifiable();

            dataProviderMoq.Setup(c => c.Put<TodoList>(It.IsAny<TodoList>()))
                .Returns(() => Task.CompletedTask)
                .Verifiable();

            subject = new TodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);

        }

        [Test]
        public void ApplyQueryAttributes_QueryNull()
        {
            subject.ApplyQueryAttributes(null);
            dataProviderMoq.Verify(e => e.Get<TodoList>(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void ApplyQueryAttributes_UnexpectedKey()
        {
            var q = new Dictionary<string, object>
            {
                {"unexpected", "value" }
            };
            subject.ApplyQueryAttributes(q);
            dataProviderMoq.Verify(e => e.Get<TodoList>(It.IsAny<int>()), Times.Never);
        }

        //var d = new Dictionary<string, object>
        //{

        //}
    }
}

