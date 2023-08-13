namespace MauiTodo;

using MauiTodo.Models;
using MauiTodo.Services;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

public partial class MainPage : ContentPage
{
    int count = 0;
    IDataProvider dataProvider;
    IShellNavigation navigation;
    public MainPage(IDataProvider dataProvider, IShellNavigation navigation)
    {
        this.dataProvider = dataProvider;
        this.navigation = navigation;
        InitializeComponent();
        Task.Run(async () =>
        {

            var d = await dataProvider.Get<Count>(0);
            count = d.Value;
            updateText();
        });
    }

    private void updateText()
    {
        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        navigation.GoToAsync("//AllTodoListsPage");
        count++;
        dataProvider.Put<Count>(new Count { Value = count });
        updateText();
    }


}


