	using System;
using MauiTodo.Services;
using MauiTodo.Models;


namespace MauiTodo.ViewModels
{
	public class AllTodoListViewModel : BaseObservable
	{
		IDataProvider dataProvider;
		
		public AllTodoListViewModel(IDataProvider dataProvider)
		{
			this.dataProvider = dataProvider;
			Task.Run(async () =>
			{
				var id = 1;
				await dataProvider.Put<Data>(new Data
				{
					Count = new Count { Value = 2 },
					AllTodoLists = new AllTodoLists
					{
						TodoLists = new List<TodoList>{
							new TodoList
							{
								Title = "bluelist",
								Items = new List<TodoItem>{
                                    new TodoItem{
                                        Id =id++,
                                        Title = $"test{id}"
                                    },
                                    new TodoItem{
                                        Id =id++,
                                        Title = $"test{id}"
                                    },
                                    new TodoItem{
                                        Id =id++,
                                        Title = $"test{id}"
                                    }
                                }
							},
                            new TodoList
                            {
                                Title = "redlist",
                                Items = new List<TodoItem>{
                                    new TodoItem{
                                        Id =id++,
                                        Title = $"junk{id}"
                                    },
                                    new TodoItem{
                                        Id =id++,
                                        Title = $"junk{id}"
                                    },
                                    new TodoItem{
                                        Id =id++,
                                        Title = $"junk{id}"
                                    }
                                }
                            }
                        }
					}
				});
                var d = await dataProvider.Get<Data>(0);
				var l = d.AllTodoLists;

				
			});
			
		}
	}
}

