using HamiltonVisualizer.Core.Contracts;
using HamiltonVisualizer.Events.EventArgs.ForFeature.FeatureEventArgs;
using HamiltonVisualizer.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HamiltonVisualizer.Core.CustomControls.WPFBorder;
internal sealed class SearchBox : Border, IUIComponent
{
    private readonly Feature _parent;

    private readonly StackPanel _container;
    private readonly TextBox _textBox;
    private readonly Button _okButton;
    private readonly Button _exitButton;

    public SearchBoxMode Mode { get; set; }
    public double ButtonBorderThickness = 1;
    public double SearchBoxBorderThickness = 1;
    public Brush OkButtonBackground => _okButton.Background;
    public Brush ExitButtonBackground => _exitButton.Background;

    public SearchBox(Feature feature)
    {
        _parent = feature;

        Grid.SetRow(this, 1);
        Grid.SetColumn(this, 0);

        StyleUIComponent();

        _container = new()
        {
            Orientation = Orientation.Horizontal,
        };

        _textBox = new TextBox()
        {
            Width = 125,
            Height = 25,
            Margin = new Thickness(10, 6, 0, 0),
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(0.5),
            Background = Brushes.Beige,
            FontSize = 15,
        };

        _textBox.KeyDown += TextBox_KeyDown;

        _okButton = new Button()
        {
            Width = 20,
            Height = 25,
            Margin = new Thickness(0, 6, 0, 0),
            BorderThickness = new Thickness(0, ButtonBorderThickness, ButtonBorderThickness, ButtonBorderThickness),
            Content = '→'
        };

        _exitButton = new Button()
        {
            Width = 20,
            Height = 25,
            Margin = new Thickness(0, 6, 0, 0),
            BorderThickness = new Thickness(0, ButtonBorderThickness, ButtonBorderThickness, ButtonBorderThickness),
            Content = 'X'
        };

        _exitButton.Click += (_, _) =>
        {
            _parent.CollapseSearchBox();
        };

        _container.Children.Add(_textBox);
        _container.Children.Add(_okButton);
        _container.Children.Add(_exitButton);

        Child = _container;
    }

    private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        switch (e.Key)
        {
            case System.Windows.Input.Key.Escape:
                _textBox.Text = "";
                Collapse();
                break;
            case System.Windows.Input.Key.Enter:
                Execute(Mode);
                Collapse();
                break;
        }
    }
    private void Execute(SearchBoxMode mode)
    {
        switch (mode)
        {
            case SearchBoxMode.Delete:
                _parent.OnDelete(new DeleteEventArgs(_textBox.Text));
                break;
            case SearchBoxMode.Find:
                _parent.OnFind(new FindEventArgs(_textBox.Text));
                break;
        }
    }

    public void StyleUIComponent()
    {
        Background = Brushes.FloralWhite;
        Margin = new Thickness(968, 0, 0, 669);
        BorderBrush = Brushes.Black;
        BorderThickness = new Thickness(SearchBoxBorderThickness, 0, 0, SearchBoxBorderThickness);
        CornerRadius = new CornerRadius(0, 0, 0, 8);
        Name = nameof(SearchBox);
        Visibility = Visibility.Collapsed; // collapse by default
    }
    public void ShowWithMode(SearchBoxMode mode)
    {
        Mode = mode;
        _okButton.Background = mode switch
        {
            SearchBoxMode.Delete => Brushes.OrangeRed,
            SearchBoxMode.Find => Brushes.WhiteSmoke,
            _ => Brushes.WhiteSmoke,
        };
        Visibility = Visibility.Visible;
        _textBox.Focus();
        _textBox.SelectAll();
    }
    public void Collapse()
    {
        Visibility = Visibility.Collapsed;
    }

}

internal enum SearchBoxMode
{
    Delete,
    Find
}
