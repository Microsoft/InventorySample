# Application Startup

Every app or web, with complex patterns involved, normally requires an initial setup to put everything in motion. In this section we will review the initial setup required to make it works.

## The Startup class

As you may know, the App class (App.xaml file in the Inventory.App project), is the first class to be executed when we launch the application. In the overriden methods `OnLaunched` and `OnActivated` of this class, we execute the intial setup and initialize the first page to load:

```csharp
    protected override async void OnLaunched(LaunchActivatedEventArgs e)
    {
        await ActivateAsync(e);
    }

    protected override async void OnActivated(IActivatedEventArgs e)
    {
        await ActivateAsync(e);
    }

    private async Task ActivateAsync(IActivatedEventArgs e)
    {
        var activationInfo = ActivationService.GetActivationInfo(e);

        var frame = Window.Current.Content as Frame;
        if (frame == null)
        {
            // Setup the application
            await Startup.ConfigureAsync();

            frame = new Frame();
            Window.Current.Content = frame;
            var shellArgs = new ShellArgs
            {
                ViewModel = activationInfo.EntryViewModel,
                Parameter = activationInfo.EntryArgs,
                UserInfo = await TryGetUserInfoAsync(e as IActivatedEventArgsWithUser)
            };
            // Navigate to the first page
            frame.Navigate(typeof(LoginView), shellArgs);
            Window.Current.Activate();
        }
        else
        {
            var navigationService = ServiceLocator.Current.GetService<INavigationService>();
            await navigationService.CreateNewViewAsync(activationInfo.EntryViewModel, activationInfo.EntryArgs);
        }
    }
```  

Calling the method `ConfigureAsync` of the `Startup` class, we are initializing everything we need to start working with the app:

 ```csharp
    static public async Task ConfigureAsync()
    {
        ServiceLocator.Configure(_serviceCollection);

        ConfigureNavigation();

        await EnsureLogDbAsync();
        await EnsureDatabaseAsync();
        await ConfigureLookupTables();

        var logService = ServiceLocator.Current.GetService<ILogService>();
        await logService.WriteAsync(Data.LogType.Information, "Startup", "Configuration", "Application Start", $"Application started.");

        ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 500));
    }
 ```

We can distinguish the following setups:

- `ServiceLocator.Configure(_serviceCollection)`: Initialize and configures the ServiceLocator to enable Dependency Injection.
- `ConfigureNavigation`: This method registers the ViewModels and the Views in order to make our *Navigation Service* works. You can check more about the *Navigation Service* [here](navigation-service.md#INavigationService-implementation).
- `EnsureDatabaseAsync`: This method makes sure that the database exists or created if there`s no one.
- `ConfigureLookupTables`: Once the database is created, we are caching the setup tables values in a `ILookupTables` service to avoid continuous access to the database.
