# Windows 10 Binding

As we previously mentioned in [MMVM chapter](mvvm.md#data-binding), Data binding is a way for your app's UI to display data, and optionally to stay in sync with that data. This is the technique we are using to communicate our View-Models with the Views.

In this section, we will see in detail the new markup XAML notation `x:Bind` and its differences and advantages compared to the traditional `Binding`.

## Binding Elements

Every binding process requires of three elements in place to ensure the communication between the UI and the Data Source:

- *Binding source*: This is the source of the data for the binding, and it can be an instance of any class that has members whose values you want to display in your UI. The View-Models are the binding source that we are using in our example app.

- *Binding target*: This is the **Dependency Property** of the control of the UI that displays the data. For example the dependency property `Text` of a control `TextBlock`.

- *Binding object*: This is the piece that transfers data values from the source to the target, and optionally from the target back to the source. The binding object is created at XAML load time from your {x:Bind} or {Binding} markup extension.

## Dependency property

The property that is the target of a data binding must be a dependency property. These properties are defined in a specific way in order to allow data biding over them. 

Most of the properties of the UI controls we use in the XAML are *Dependency Properties*. You can create your own *Dependency Properties* for your custom controls, but in order to define them, your class has to be a `DependencyObject`. This is an example of a *Dependency Property* that we have defined in the **Inventory Sample App**:

```c#
public CustomerModel Model
{
    get { return (CustomerModel)GetValue(ModelProperty); }
    set { SetValue(ModelProperty, value); }
}

public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", 
                            typeof(CustomerModel), 
                            typeof(CustomerDetails), 
                            new PropertyMetadata(null));
```

Once we have the *Dependency Property* defined in our custom control, we can bind data to it in our XAML views in this way:

```xml
<views:CustomerDetails 
    Model="{x:Bind ViewModel.SelectedCustomer, Mode=OneWay}" />
```

We could declare *Dependency Properties* even more complex, for instance, we can have a callback mechanism when the value of the property changes:

```c#
public bool IsEditMode
{
    get { return (bool)GetValue(IsEditModeProperty); }
    set { SetValue(IsEditModeProperty, value); }
}

public static readonly DependencyProperty IsEditModeProperty = 
    DependencyProperty.Register("IsEditMode", 
                                typeof(bool), 
                                typeof(CustomerDetails), 
                                new PropertyMetadata(false, IsEditModeChanged));

private static void IsEditModeChanged(DependencyObject d, 
                                      DependencyPropertyChangedEventArgs e)
{
    var control = d as CustomerDetails;
    control.UpdateEditMode();
}       
```

## Difference between x:Bind and Binding

There are two kinds of binding, and they're both typically declared in UI markup. You can choose to use either the `{x:Bind}` markup extension or the `{Binding}` markup extension. And you can even use a mixture of the two in the same app—even on the same UI element. 

`{x:Bind}` executes special-purpose code, which it generates at compile-time. `{Binding}` uses general-purpose runtime object inspection. Consequently, `{x:Bind}` has great performance and provides compile-time validation of your binding expressions. It supports debugging by enabling you to set breakpoints in the code files that are generated as the partial class for your page.

Another difference is their default binding mode. Check [MVVM: Types of bindings](mvvm.md#data-binding) for more information. `{Binding}` has a deafult binding mode of *OneWay* while the default binding for  `{x:Bind}` is *OneTime*. 

## Binding markup extension

`{Binding}` markup extension, assumes by default, that the binding object to use is the `DataContext` of the page. So we'll set the DataContext of our page to be an instance of our binding source class (of type HostViewModel in this case).

```xml
<Page xmlns:viewmodel="using:QuizGame.ViewModel" ... >
    <Page.DataContext>
        <viewmodel:HostViewModel/>
    </Page.DataContext>
    ...
    <Button Content="{Binding Path=NextButtonText}" ... />
</Page>
```

The `{Binding}` markup extension is converted at XAML load time into an instance of the `Binding` class. And this binding object created is the one reponsable of tracking the changes between the data source and the dependency target property.

## x:Bind markup extension

This is the type of binding we are using in most parts of the Inventory Sample app.

Because `{x:Bind}` uses generated code to achieve its benefits, it requires type information at compile time. This means that you cannot bind to properties where you do not know the type ahead of time. Because of this, you cannot use `{x:Bind}` with the `DataContext` property which is of type Object, and is also subject to change at run time. The `{x:Bind}` markup extension—new for Windows 10—is an alternative to `{Binding}`. `{x:Bind}` lacks some features of `{Binding}`, but it runs in less time and less memory than `{Binding}` and supports better debugging.

`{x:Bind}` does not use the `DataContext` as a default source, instead, it uses the page or user control itself. One recommended practice is to declare in our pages a property `ViewModel` to bind the data in our UI. This is an example of how we declare the ViewModel property and how it's used in XAML:

```c#
 public sealed partial class CustomersView : Page
{
    public CustomersView()
    {
        InitializeViewModel();
        InitializeComponent();
    }

    public CustomersViewModel ViewModel { get; private set; }

    private void InitializeViewModel()
    {
        ViewModel = new CustomersViewModel(new DataProviderFactory());
        ViewModel.UpdateBindings += OnUpdateBindings;
    }
}
```

```xml
<!--x:Bind example-->
<controls:Section 
    Margin="6,6,6,3" 
    Header="{x:Bind ViewModel.QuotedQuery}">
</controls:Section>
```

### Functions in binding path

Starting in Windows 10, `{x:Bind}` allow us to bind functions directly in the XAML files. This enables:

- A simpler way to achieve value conversion
- A way to resolve Bindings that depend on more than one parameter.

We can bind functions located in the code behind of the page or control, and even static functions if we use the XMLNamespace:ClassName.MethodName syntax. Let's see examples of how to use the functions in our Bindings:

```xml
<TextBlock Grid.Column="2" Text="{x:Bind UIHelper.ToShortDate(OrderDate)}"/>
```
Every function that we bind has to:

- Be accesible from the code behind of Page / Control.
- The arguments type needs to match the data being passed in.
- The return type of the function needs to match the Type of the property we are binding to.

#### Two way functions binding

It is possible to have two way binding scenario using functions, and to put it in place we need to indicate the `BindBack`binding property. This would be an example of how to do it:

```xml
<TextBlock Text="{x:Bind a.MyFunc(b), BindBack=a.MyFunc2}" />
``` 
Note that we are not indicating the argument of the `BindBack` function, but it has to have one parameter of the same type of the property that we are binding to. In this case we are passing as parameter the `Text` of the `TextBlock` control.

### Event Binding

It allows you to bind the handler of an event, rather than having it declared in the code-behind . For example:

```xml
<Button Click="rootFrame.GoForward" />
```

The target method has to:
- Match the signature of the event
- OR have no parameters
- OR have the same number of parameters of types that are assignable from the types of the event parameters.

Another characteristic of Event Binding is that the binding expression is evaluated when the event occurs, meaning, unlike property bindings, it doesn’t track changes to the model.

### Updating bindings

Pages and user controls that include the `{x:Bind}` markup extention will have a "Bindings" property in the generated code. This includes the following methods:

- `Update()`: This will update the values of all compiled bindings. Any one-way/Two-Way bindings will have the listeners hooked up to detect changes.
- `Initialize()`: If the bindings have not already been initialized, then it will call Update() to initialize the bindings
- `StopTracking()`: This will unhook all listeners created for one-way and two-way bindings. They can be re-initialized using the Update() method.


## Value Converters

Both `{x:Bind}` and `{Binding}` allow *Value Converters*. This is an important resource that needs to be explained as it's frecuently used in Windows 10 apps development. 

Sometimes we need to display, for instance, some data from the ViewModel in a specific format but that information is irrelevant at the ViewModel level. For example, imagine that we have a `decimal` property in the ViewModel and we want to display that decimal with a specific format in our View. To accomplish this and without adding this unnecesary convertion in our ViewModel, we can use implementations of `IValueConverter`.

We will create a new class implementing the `IValueConverter` interface:

```c#
public sealed class DecimalConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is decimal m)
        {
            return m == 0m ? "" : m.ToString("0.00");
        }
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value != null)
        {
            if (Decimal.TryParse(value.ToString(), out decimal m))
            {
                return m;
            }
        }
        return 0m;
    }
}
```

And once the new class is created we can use it in our XAML files in the following way:

```xml
<controls:LabelTextBox Label="YearlyIncome" 
    Text="{x:Bind Item.YearlyIncome, 
           Mode=TwoWay, 
           Converter={StaticResource DecimalConverter}}" 
    ValueType="Decimal" />
```

Last and before starting using our *ValueConverters* we need to declare them as Resources to be used. You can do this at the control or page level or declare them globally in the App.xaml file:

```xml
<Application
    x:Class="VanArsdel.Inventory.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:converters="using:VanArsdel.Inventory.Converters"
    RequestedTheme="Light">

    <Application.Resources>
        <ResourceDictionary>
            <converters:BoolToVisibilityConverter x:Key="BoolToVisibilityConverter" FalseValue="Collapsed" TrueValue="Visible" />
            <converters:BoolToVisibilityConverter x:Key="InverseBoolToVisibilityConverter" FalseValue="Visible" TrueValue="Collapsed" />

            <converters:Int16Converter x:Key="Int16Converter" />
            <converters:Int32Converter x:Key="Int32Converter" />
            <converters:Int64Converter x:Key="Int64Converter" />
            <converters:DecimalConverter x:Key="DecimalConverter" />
            <converters:DoubleConverter x:Key="DoubleConverter" />

        </ResourceDictionary>
    </Application.Resources>

</Application>
```

Please note, that we need to include the namespace where they are defined in order to expose them with the following instruction:

`xmlns:converters="using:VanArsdel.Inventory.Converters"`


## Summary

We have 2 binding mechanisims: `{x:Bind}` and `{Binding}`.

The new Windows 10 markup extension `{x:Bind}`, is an alternative to `{Binding}`. `{x:Bind}` lacks some features of `{Binding}`, but it runs in less time and less memory than `{Binding}` and supports better debugging. With `{x:Bind}` we can also bind functions and events. 

*Value Converters* are great tools to be used in our UI binding expressions. Their function is to apply some format over the original values of the ViewModels and display them in our Views. 
