using MauiIcons.Material;
using MauiTodo.Services;
using MauiTodo.ViewModels;
using MauiTodo.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace MauiTodo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMaterialMauiIcons()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("materialsymbols.ttf", "materialsymbols");
            })
            .ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android => android
                    //.OnActivityResult((activity, requestCode, resultCode, data) => LogEvent(nameof(AndroidLifecycle.OnActivityResult), requestCode.ToString()))
                    //.OnStart((activity) => LogEvent(nameof(AndroidLifecycle.OnStart)))
                    //.OnCreate((activity, bundle) => LogEvent(nameof(AndroidLifecycle.OnCreate)))
                    //.OnBackPressed((activity) => LogEvent(nameof(AndroidLifecycle.OnBackPressed)) && false)
                    .OnStop((activity) => LeaveEvent())
                    .OnPause((activity) => LeaveEvent())
                        );
#endif
#if IOS
                    events.AddiOS(ios => ios
                        //.OnActivated((app) => LogEvent(nameof(iOSLifecycle.OnActivated)))
                        //.OnResignActivation((app) => LogEvent(nameof(iOSLifecycle.OnResignActivation)))
                        .DidEnterBackground((app) => LeaveEvent())
                        .WillTerminate((app) => LeaveEvent()));
#endif

                // save data when app is backgrounded
                static bool LeaveEvent()
                {

#if ANDROID
                    var Current = MauiApplication.Current.Services;
#elif IOS || MACCATALYST
                    var Current = MauiUIApplicationDelegate.Current.Services;
#endif
#if IOS || MACCATALYST || ANDROID
                    var dataProvider = Current.GetService<IDataProvider>();
                    dataProvider.Save();
                    return true;
#else
                    return false;
#endif
                }
            });
        builder.Services.AddSingleton<IDataProvider, DataProvider>();
        builder.Services.AddSingleton<ILog, Log>();
        //builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<IDialogService, DialogService>();
        builder.Services.AddTransient<IShellNavigation, ShellNavigation>();
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

