using MauiTodo.Services;
using MauiTodo.ViewModels;
using MauiTodo.Views;
using Microsoft.Extensions.Logging;

namespace MauiTodo;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.Services.AddTransient<IDataProvider, DataProvider>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<AllTodoListsPage>();
        builder.Services.AddTransient<AllTodoListViewModel>();
        builder.Services.AddTransient<TodoListPage>();
        builder.Services.AddTransient<TodoListViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

