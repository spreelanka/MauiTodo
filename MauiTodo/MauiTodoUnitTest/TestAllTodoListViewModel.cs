using System;
using System.Collections.ObjectModel;
using MauiTodo.Models;
using MauiTodo.Services;
using MauiTodo.ViewModels;
using MauiTodo.Views;
using Moq;
using Newtonsoft.Json;

namespace MauiTodoUnitTest
{
    public class TestAllTodoListViewModel
    {
        AllTodoListViewModel subject;
        Mock<ILog> logMoq;
        Mock<IDataProvider> dataProviderMoq;
        Mock<IShellNavigation> navigationMoq;

        Task<Data> Get_MockResponse()
        {
            var data = new Data
            {
                AllTodoLists = new AllTodoLists
                {
                    TodoLists = new ObservableCollection<TodoList>{
                        new TodoList
                        {
                            Id=1,
                            Title = "firstlist",
                            Items = new ObservableCollection<TodoItem>{
                                new TodoItem{
                                    Id = 11,
                                    Title = $"item1"
                                }
                            }
                        },
                        new TodoList
                        {
                            Id=2,
                            Title = "secondlist",
                            Items = new ObservableCollection<TodoItem>{
                                new TodoItem{
                                    Id = 22,
                                    Title = $"item1"
                                },
                                new TodoItem{
                                    Id = 33,
                                    Title = $"item2"
                                }
                            }
                        }
                    }
                }
            };

            return Task.FromResult(data);
        }


        [SetUp]
        public void Setup()
        {
            logMoq = new Mock<ILog>();
            dataProviderMoq = new Mock<IDataProvider>();
            navigationMoq = new Mock<IShellNavigation>();
            dataProviderMoq.Setup(c => c.Get<Data>(It.IsAny<int>()))
                .Returns((int id) => Get_MockResponse())
                .Verifiable();

            dataProviderMoq.Setup(c => c.Put<TodoList>(It.IsAny<TodoList>()))
                .Returns(() => Task.CompletedTask)
                .Verifiable();

            subject = new AllTodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);

        }

        [Test]
        public async Task Constructor()
        {
            var getTcs = new TaskCompletionSource<bool>();
            dataProviderMoq = new Mock<IDataProvider>();
            dataProviderMoq.Setup(c => c.Get<Data>(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    getTcs.TrySetResult(true);
                    return Get_MockResponse();
                })
                .Verifiable();
            subject = new AllTodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);
            await getTcs.Task;
            var expected = JsonConvert.SerializeObject(Get_MockResponse().Result);
            dataProviderMoq.Verify(e => e.Get<Data>(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(expected, JsonConvert.SerializeObject(subject.Data));
        }

        [Test]
        public void ApplyQueryAttributes_QueryNull()
        {
            Dictionary<string, object> q = null;
            subject.ApplyQueryAttributes(q);
        }

        [Test]
        public void ApplyQueryAttributes_UnexpectedKey()
        {
            var q = new Dictionary<string, object>
            {
                {"unexpected", "value" }
            };
            subject.ApplyQueryAttributes(q);
        }

        [Test]
        public async Task ApplyQueryAttributes_DeleteIdNull()
        {
            var q = new Dictionary<string, object>
            {
                {"Delete", null }
            };
            Assert.Throws<ArgumentException>(() => subject.ApplyQueryAttributes(q));

        }

        [Test]
        public async Task ApplyQueryAttributes_DeleteIdInvalid()
        {
            var q = new Dictionary<string, object>
            {
                {"Delete", "invalid" }
            };
            Assert.Throws<ArgumentException>(() => subject.ApplyQueryAttributes(q));
        }

        [Test]
        public async Task ApplyQueryAttributes_DeleteIdDoesNotExist()
        {
            var q = new Dictionary<string, object>
            {
                {"Delete", "55555" }
            };
            var getTcs = new TaskCompletionSource<bool>();
            var putTcs = new TaskCompletionSource<bool>();
            var saveTcs = new TaskCompletionSource<bool>();
            dataProviderMoq = new Mock<IDataProvider>();
            dataProviderMoq.Setup(c => c.Get<Data>(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    getTcs.TrySetResult(true);
                    return Get_MockResponse();
                })
                .Verifiable();

            dataProviderMoq.Setup(c => c.Put<Data>(It.IsAny<Data>()))
                .Returns(() =>
                {
                    putTcs.SetResult(true);
                    return Task.CompletedTask;
                })
                .Verifiable();

            dataProviderMoq.Setup(c => c.Save())
                .Returns(() =>
                {
                    saveTcs.SetResult(true);
                    return Task.CompletedTask;
                })
                .Verifiable();
            subject = new AllTodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);
            subject.ApplyQueryAttributes(q);

            if (!Task.WaitAll(new[] { getTcs.Task, putTcs.Task, saveTcs.Task }, 1000))
            {
                Assert.True(getTcs.Task.IsCompleted, "dataProvider.Get() not called");
            }

            dataProviderMoq.Verify(e => e.Put<Data>(It.IsAny<Data>()), Times.Never);
            dataProviderMoq.Verify(e => e.Get<Data>(It.IsAny<int>()), Times.Exactly(2));
            dataProviderMoq.Verify(e => e.Save(), Times.Never);
        }

        [Test]
        public async Task ApplyQueryAttributes_DeleteIdValid()
        {
            var id = 1;
            var query = new Dictionary<string, object>
            {
                {"Delete", id }
            };

            var expectedSerialized = JsonConvert.SerializeObject(new Data
            {
                AllTodoLists = new AllTodoLists
                {
                    TodoLists = new ObservableCollection<TodoList>{
                        new TodoList
                        {
                            Id=2,
                            Title = "secondlist",
                            Items = new ObservableCollection<TodoItem>{
                                new TodoItem{
                                    Id = 22,
                                    Title = $"item1"
                                },
                                new TodoItem{
                                    Id = 33,
                                    Title = $"item2"
                                }
                            }
                        }
                    }
                }
            });
            var getTcs = new TaskCompletionSource<bool>();
            var putTcs = new TaskCompletionSource<bool>();
            var saveTcs = new TaskCompletionSource<bool>();
            dataProviderMoq = new Mock<IDataProvider>();
            dataProviderMoq.Setup(c => c.Get<Data>(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    getTcs.TrySetResult(true);
                    return Get_MockResponse();
                })
                .Verifiable();

            dataProviderMoq.Setup(c => c.Put<Data>(It.IsAny<Data>()))
                .Returns(() =>
                {
                    putTcs.SetResult(true);
                    return Task.CompletedTask;
                })
                .Verifiable();

            dataProviderMoq.Setup(c => c.Save())
                .Returns(() =>
                {
                    saveTcs.SetResult(true);
                    return Task.CompletedTask;
                })
                .Verifiable();

            subject = new AllTodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);

            subject.ApplyQueryAttributes(query);


            if (!Task.WaitAll(new[] { getTcs.Task, putTcs.Task, saveTcs.Task }, 2000))
            {
                Assert.True(getTcs.Task.IsCompleted, "dataProvider.Get() not called");
                Assert.True(putTcs.Task.IsCompleted, "dataProvider.Put() not called");
                Assert.True(saveTcs.Task.IsCompleted, "dataProvider.Save() not called");
            }

            dataProviderMoq.Verify(e => e.Put<Data>(It.IsAny<Data>()), Times.Once);
            dataProviderMoq.Verify(e => e.Get<Data>(It.IsAny<int>()), Times.Exactly(2));
            dataProviderMoq.Verify(e => e.Save(), Times.Once);
            Assert.AreEqual(expectedSerialized, JsonConvert.SerializeObject(subject.Data));
        }

        [Test]
        public async Task AddTodoListCommand_Default()
        {
            var before = new Data
            {
                AllTodoLists = new AllTodoLists
                {
                    TodoLists = new ObservableCollection<TodoList>{
                        new TodoList
                        {
                            Id=2,
                            Title = "secondlist",
                            Items = new ObservableCollection<TodoItem>{
                                new TodoItem{
                                    Id = 22,
                                    Title = $"item1"
                                },
                                new TodoItem{
                                    Id = 33,
                                    Title = $"item2"
                                }
                            }
                        }
                    }
                }
            };
            var putExpected = new TodoList { Title = "new todolist" };
            var after = new Data
            {
                AllTodoLists = new AllTodoLists
                {
                    TodoLists = new ObservableCollection<TodoList>{
                        new TodoList { Title = "new todolist", Id = 3 },
                        new TodoList
                        {
                            Id=2,
                            Title = "secondlist",
                            Items = new ObservableCollection<TodoItem>{
                                new TodoItem{
                                    Id = 22,
                                    Title = $"item1"
                                },
                                new TodoItem{
                                    Id = 33,
                                    Title = $"item2"
                                }
                            }
                        }
                    }
                }
            };
            var expectedSerialized = JsonConvert.SerializeObject(after);
            var getTcs = new TaskCompletionSource<bool>();
            var putTcs = new TaskCompletionSource<bool>();
            var saveTcs = new TaskCompletionSource<bool>();

            dataProviderMoq = new Mock<IDataProvider>();
            dataProviderMoq.Setup(c => c.Get<Data>(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    getTcs.TrySetResult(true);
                    return Task.FromResult(after);
                })
                .Verifiable();

            dataProviderMoq.Setup(c => c.Put<TodoList>(It.IsAny<TodoList>()))
                .Returns((TodoList newList) =>
                {
                    Assert.AreEqual(JsonConvert.SerializeObject(putExpected), JsonConvert.SerializeObject(newList));

                    putTcs.SetResult(true);
                    return Task.CompletedTask;
                })
                .Verifiable();

            dataProviderMoq.Setup(c => c.Save())
                .Returns(() =>
                {
                    saveTcs.SetResult(true);
                    return Task.CompletedTask;
                })
                .Verifiable();

            subject = new AllTodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);
            await subject.AddTodoListCommand.ExecuteAsync("");

            if (!Task.WaitAll(new[] { getTcs.Task, putTcs.Task, saveTcs.Task }, 2000))
            {
                Assert.True(getTcs.Task.IsCompleted, "dataProvider.Get() not called");
                Assert.True(putTcs.Task.IsCompleted, "dataProvider.Put() not called");
                Assert.True(saveTcs.Task.IsCompleted, "dataProvider.Save() not called");
            }

            dataProviderMoq.Verify(e => e.Put<TodoList>(It.IsAny<TodoList>()), Times.Once);
            dataProviderMoq.Verify(e => e.Get<Data>(It.IsAny<int>()), Times.Exactly(2));
            dataProviderMoq.Verify(e => e.Save(), Times.Once);
            Assert.AreEqual(expectedSerialized, JsonConvert.SerializeObject(subject.Data));
        }

        class ListViewMock : ListView
        {
            protected override void OnBindingContextChanged()
            {
            }
        }

        [Test]
        public async Task ListView_ItemSelected_SelectedItemValid()
        {
            var selectedItem = new TodoList { Id = 222 };
            var navigationExpected = $"/{nameof(TodoListPage)}?{nameof(TodoList.Id)}={selectedItem.Id}";

            navigationMoq = new Mock<IShellNavigation>();
            navigationMoq.Setup(c => c.GoToAsync(It.IsAny<ShellNavigationState>()))
                .Returns((Func<ShellNavigationState, Task>)(async (state) =>
                {
                    Assert.AreEqual(navigationExpected, state.Location.OriginalString);
                }))
                .Verifiable();
            logMoq = new Mock<ILog>();
            logMoq.Setup(c => c.Error(It.IsAny<string>(), It.IsAny<Exception>()))
                .Verifiable();

            subject = new AllTodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);

            subject.ListView_ItemSelected(null, new SelectedItemChangedEventArgs(selectedItem, 0));

            navigationMoq.Verify(e => e.GoToAsync(It.IsAny<ShellNavigationState>()), Times.Once);
            logMoq.Verify(e => e.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Test]
        public async Task ListView_ItemSelected_SelectedItemNull()
        {
            TodoList selectedItem = null;

            navigationMoq = new Mock<IShellNavigation>();
            navigationMoq.Setup(c => c.GoToAsync(It.IsAny<ShellNavigationState>()))
                .Returns((Func<ShellNavigationState, Task>)(async (state) =>
                {
                }))
                .Verifiable();
            logMoq = new Mock<ILog>();
            logMoq.Setup(c => c.Error(It.IsAny<string>(), It.IsAny<Exception>()))
                .Verifiable();

            subject = new AllTodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);
            subject.ListView_ItemSelected(null, new SelectedItemChangedEventArgs(selectedItem, 0));

            navigationMoq.Verify(e => e.GoToAsync(It.IsAny<ShellNavigationState>()), Times.Never);
            logMoq.Verify(e => e.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Test]
        public async Task ListView_ItemSelected_NotSupported()
        {
            var selectedItem = "unsupported object type";

            navigationMoq = new Mock<IShellNavigation>();
            navigationMoq.Setup(c => c.GoToAsync(It.IsAny<ShellNavigationState>()))
                .Returns((Func<ShellNavigationState, Task>)(async (state) =>
                {
                }))
                .Verifiable();
            logMoq = new Mock<ILog>();
            logMoq.Setup(c => c.Error(It.IsAny<string>(), It.IsAny<Exception>()))
                .Verifiable();

            subject = new AllTodoListViewModel(dataProviderMoq.Object, logMoq.Object, navigationMoq.Object);
            subject.ListView_ItemSelected(null, new SelectedItemChangedEventArgs(selectedItem, 0));

            navigationMoq.Verify(e => e.GoToAsync(It.IsAny<ShellNavigationState>()), Times.Never);
            logMoq.Verify(e => e.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }
    }
}

