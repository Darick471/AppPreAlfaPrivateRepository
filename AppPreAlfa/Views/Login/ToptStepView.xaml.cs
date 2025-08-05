using AppPreAlfa.ViewModels.Login;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using ReactiveUI;
using System.Reactive.Disposables;

namespace AppPreAlfa.Views.Login;

public sealed partial class ToptStepView : Page, IViewFor<ToptStepViewModel>
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(ToptStepViewModel),
            typeof(ToptStepView),
            new PropertyMetadata(null, OnViewModelChanged));

    public ToptStepViewModel ViewModel
    {
        get => (ToptStepViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (ToptStepViewModel)value;
    }

    private readonly TextBox[] _codeTextBoxes;

    public ToptStepView()
    {
        this.InitializeComponent();
        _codeTextBoxes = new[] { TextBox1, TextBox2, TextBox3, TextBox4, TextBox5, TextBox6 };

        this.WhenActivated(disposable =>
        {
            // Enlazar el comando al botón
            this.BindCommand(ViewModel, vm => vm.ValidateToptCommand, v => v.IniciarSesionButton)
                .DisposeWith(disposable);

            for (int i = 0; i < _codeTextBoxes.Length; i++)
            {
                var currentTextBox = _codeTextBoxes[i];
                int currentIndex = i;

                currentTextBox.TextChanged += (s, e) =>
                {
                    // se remplaza el array para que ReactiveUI detecte el cambio.
                    if (ViewModel != null)
                    {
                        var newParts = (string[])ViewModel.CodeParts.Clone();
                        newParts[currentIndex] = currentTextBox.Text;
                        ViewModel.CodeParts = newParts;
                    }

                    if (!string.IsNullOrEmpty(currentTextBox.Text) && currentIndex < _codeTextBoxes.Length - 1)
                    {
                        _codeTextBoxes[currentIndex + 1].Focus(FocusState.Programmatic);
                    }
                };

                currentTextBox.KeyDown += (s, e) =>
                {
                    if (e.Key == Windows.System.VirtualKey.Back && string.IsNullOrEmpty(currentTextBox.Text) && currentIndex > 0)
                    {
                        _codeTextBoxes[currentIndex - 1].Focus(FocusState.Programmatic);
                    }
                };
            }
        });
    }

    // Este método asegura que la vista use el ViewModel correcto en cuanto se le asigne.
    private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Page page)
        {
            page.DataContext = e.NewValue;
        }
    }
}
