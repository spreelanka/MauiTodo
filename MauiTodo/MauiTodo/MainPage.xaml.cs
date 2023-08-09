namespace MauiTodo;

using GoogleGson;
using MauiTodo.Models;
using MauiTodo.Services;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

public partial class MainPage : ContentPage
{
	int count = 0;
    IDataProvider dataProvider;

    public MainPage(IDataProvider dataProvider)
	{
        this.dataProvider = dataProvider;
		InitializeComponent();
		Task.Run(async () =>
		{
            var d = await dataProvider.Get<Data>(0);
            count = d.Count.Value;
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
        Shell.Current.GoToAsync("//AllTodoListsPage");
        count++;
        dataProvider.Put<Data>(new Data { Count = new Count { Value = count } });
        updateText();
	}
}


