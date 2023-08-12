using System;
using System.Collections.ObjectModel;
using MauiTodo.Models;
using MauiTodo.Services;
using MauiTodo.ViewModels;
using Moq;
using Newtonsoft.Json;

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
            Dictionary<string, object> q = null;
            Assert.Throws<ArgumentException>(() => subject.ApplyQueryAttributes(q));
            dataProviderMoq.Verify(e => e.Get<TodoList>(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void ApplyQueryAttributes_UnexpectedKey()
        {
            var q = new Dictionary<string, object>
            {
                {"unexpected", "value" }
            };
            Assert.Throws<ArgumentException>(() => subject.ApplyQueryAttributes(q));
            dataProviderMoq.Verify(e => e.Get<TodoList>(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task ApplyQueryAttributes_IdKeyNull()
        {
            var q = new Dictionary<string, object>
            {
                {nameof(TodoList.Id), null }
            };
            Assert.Throws<ArgumentException>(() => subject.ApplyQueryAttributes(q));
            dataProviderMoq.Verify(e => e.Get<TodoList>(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task ApplyQueryAttributes_IdValid()
        {
            var id = 1;
            var q = new Dictionary<string, object>
            {
                {nameof(TodoList.Id), id }
            };
            Task<TodoList> expected = Task.FromResult(new TodoList());
            dataProviderMoq = new Mock<IDataProvider>();
            dataProviderMoq.Setup(c => c.Get<TodoList>(It.IsAny<int>()))
                .Returns((int id) => expected = Get_MockResponse(id, 3))
                .Verifiable();

            dataProviderMoq.Setup(c => c.Put<TodoList>(It.IsAny<TodoList>()))
                .Returns(() => Task.CompletedTask)
                .Verifiable();


            subject = new TodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);

            var todoListActualTcs = new TaskCompletionSource<TodoList>();
            subject.PropertyChanged += (object? sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                todoListActualTcs.SetResult((sender as TodoListViewModel)?.TodoList);
            };

            subject.ApplyQueryAttributes(q);
            var todoListActual = await todoListActualTcs.Task;
            dataProviderMoq.Verify(e => e.Get<TodoList>(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(await expected, subject.TodoList);
        }

        [Test]
        public async Task DeleteTodoList_ValidId()
        {
            var id = 1;
            var q = new Dictionary<string, object>
            {
                {nameof(TodoList.Id), id }
            };
            var expected = $"..?Delete={id}";

            navigationMoq = new Mock<IShellNavigation>();
            navigationMoq.Setup(c => c.GoToAsync(It.IsAny<ShellNavigationState>()))
                .Returns((Func<ShellNavigationState, Task>)(async (state) =>
                {
                    Assert.AreEqual(expected, state.Location.OriginalString);
                }))
                .Verifiable();

            subject = new TodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);
            subject.TodoList = await Get_MockResponse(1);
            await subject.DeleteTodoListCommand.ExecuteAsync("");
            navigationMoq.Verify(e => e.GoToAsync(It.IsAny<ShellNavigationState>()), Times.Once);
        }

        [Test]
        public async Task DeleteTodoList_TodoListNullThrows()
        {
            var id = 1;
            var q = new Dictionary<string, object>
            {
                {nameof(TodoList.Id), id }
            };

            navigationMoq = new Mock<IShellNavigation>();
            navigationMoq.Setup(c => c.GoToAsync(It.IsAny<ShellNavigationState>()))
                .Returns((Func<ShellNavigationState, Task>)(async (state) =>
                {
                }))
                .Verifiable();

            subject = new TodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);
            subject.TodoList = null;
            Assert.ThrowsAsync<NullReferenceException>(() => subject.DeleteTodoListCommand.ExecuteAsync(""));
            navigationMoq.Verify(e => e.GoToAsync(It.IsAny<ShellNavigationState>()), Times.Never);
        }

        [Test]
        public async Task DeleteTodoItem_Last()
        {
            var todoItemId = 1;

            var todoListBefore = new TodoList
            {
                Id = 1,
                Title = "test list",
                Items = new ObservableCollection<TodoItem>
                {
                    new TodoItem
                    {
                        Id = 333,
                        Title = $"item1"
                    },
                        new TodoItem
                    {
                        Id = 222,
                        Title = $"item2"
                    },
                          new TodoItem
                    {
                        Id = todoItemId,
                        Title = $"item3"
                    }
                }
            };

            var todoListExpectedAfterSerialized = JsonConvert.SerializeObject(new TodoList
            {
                Id = 1,
                Title = "test list",
                Items = new ObservableCollection<TodoItem>
                {
                    new TodoItem
                    {
                        Id = 333,
                        Title = $"item1"
                    },
                        new TodoItem
                    {
                        Id = 222,
                        Title = $"item2"
                    }
                }
            });

            dataProviderMoq = new Mock<IDataProvider>();
            dataProviderMoq.Setup(c => c.Put<TodoList>(It.IsAny<TodoList>()))
                .Returns((Func<TodoList, Task>)(async (todoList) =>
                {
                    Assert.AreEqual(todoListExpectedAfterSerialized, JsonConvert.SerializeObject(todoList));
                }))
                .Verifiable();

            subject = new TodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);
            subject.TodoList = todoListBefore;
            await subject.DeleteTodoItemCommand.ExecuteAsync(todoItemId);
            Assert.AreEqual(todoListExpectedAfterSerialized, JsonConvert.SerializeObject(subject.TodoList));

            dataProviderMoq.Verify(e => e.Put<TodoList>(It.IsAny<TodoList>()), Times.Once);
        }

        [Test]
        public async Task DeleteTodoItem_First()
        {
            var todoItemId = 1;

            var todoListBefore = new TodoList
            {
                Id = 1,
                Title = "test list",
                Items = new ObservableCollection<TodoItem>
                {
                    new TodoItem
                    {
                        Id = todoItemId,
                        Title = $"item1"
                    },
                    new TodoItem
                    {
                        Id = 333,
                        Title = $"item2"
                    },
                        new TodoItem
                    {
                        Id = 222,
                        Title = $"item3"
                    }
                }
            };

            var todoListExpectedAfterSerialized = JsonConvert.SerializeObject(new TodoList
            {
                Id = 1,
                Title = "test list",
                Items = new ObservableCollection<TodoItem>
                {
                    new TodoItem
                    {
                        Id = 333,
                        Title = $"item2"
                    },
                        new TodoItem
                    {
                        Id = 222,
                        Title = $"item3"
                    }
                }
            });

            dataProviderMoq = new Mock<IDataProvider>();
            dataProviderMoq.Setup(c => c.Put<TodoList>(It.IsAny<TodoList>()))
                .Returns((Func<TodoList, Task>)(async (todoList) =>
                {
                    Assert.AreEqual(todoListExpectedAfterSerialized, JsonConvert.SerializeObject(todoList));
                }))
                .Verifiable();

            subject = new TodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);
            subject.TodoList = todoListBefore;
            await subject.DeleteTodoItemCommand.ExecuteAsync(todoItemId);
            Assert.AreEqual(todoListExpectedAfterSerialized, JsonConvert.SerializeObject(subject.TodoList));

            dataProviderMoq.Verify(e => e.Put<TodoList>(It.IsAny<TodoList>()), Times.Once);
        }

        [Test]
        public async Task GoBack_Valid()
        {
            var expectedQuery = "..?q=1";

            navigationMoq = new Mock<IShellNavigation>();
            navigationMoq.Setup(c => c.GoToAsync(It.IsAny<ShellNavigationState>()))
                .Returns((Func<ShellNavigationState, Task>)(async (state) =>
                {
                    Assert.AreEqual(expectedQuery, state.Location.OriginalString);
                }))
                .Verifiable();

            var expectedTodoList = await Get_MockResponse(1);
            dataProviderMoq = new Mock<IDataProvider>();
            dataProviderMoq.Setup(c => c.Put<TodoList>(It.IsAny<TodoList>()))
                .Returns((Func<TodoList, Task>)(async (todoList) =>
                {
                    Assert.AreEqual(expectedTodoList, todoList);
                }))
                .Verifiable();

            subject = new TodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);
            subject.TodoList = expectedTodoList;
            await subject.GoBackCommand.ExecuteAsync("");

            dataProviderMoq.Verify(e => e.Put<TodoList>(It.IsAny<TodoList>()), Times.Once);
            navigationMoq.Verify(e => e.GoToAsync(It.IsAny<ShellNavigationState>()), Times.Once);
        }

        [Test]
        public async Task AddTodoItem_ExistingList()
        {
            var expectedTodoItem = new TodoItem { Id = 334 };

            var todoListBefore = new TodoList
            {
                Id = 1,
                Title = "test list",
                Items = new ObservableCollection<TodoItem>
                {
                    new TodoItem
                    {
                        Id = 1,
                        Title = $"item1"
                    },
                    new TodoItem
                    {
                        Id = 333,
                        Title = $"item2"
                    },
                        new TodoItem
                    {
                        Id = 222,
                        Title = $"item3"
                    }
                }
            };

            var todoListExpectedAfterSerialized = JsonConvert.SerializeObject(new TodoList
            {
                Id = 1,
                Title = "test list",
                Items = new ObservableCollection<TodoItem>
                {
                    expectedTodoItem,
                    new TodoItem
                    {
                        Id = 1,
                        Title = $"item1"
                    },
                    new TodoItem
                    {
                        Id = 333,
                        Title = $"item2"
                    },
                        new TodoItem
                    {
                        Id = 222,
                        Title = $"item3"
                    }
                }
            });

            dataProviderMoq = new Mock<IDataProvider>();
            dataProviderMoq.Setup(c => c.Put<TodoList>(It.IsAny<TodoList>()))
                .Returns((Func<TodoList, Task>)(async (todoList) =>
                {
                    Assert.AreEqual(todoListExpectedAfterSerialized, JsonConvert.SerializeObject(todoList));
                }))
                .Verifiable();

            dataProviderMoq.Setup(c => c.Save())
                .Verifiable();

            subject = new TodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);
            subject.TodoList = todoListBefore;
            await subject.AddTodoItemCommand.ExecuteAsync("");
            Assert.AreEqual(todoListExpectedAfterSerialized, JsonConvert.SerializeObject(subject.TodoList));

            dataProviderMoq.Verify(e => e.Put<TodoList>(It.IsAny<TodoList>()), Times.Once);
            dataProviderMoq.Verify(e => e.Save(), Times.Once);
        }

    }
}

