namespace MauiTodo;

public partial class App : Application
{
	public App(MainPage mainPage)
	{
		InitializeComponent();

		MainPage = mainPage;// new AppShell();
	}
}

