using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Testing_WPF
{

    //ToDo: add more guidance
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ImageHandling ImageHandler { get; set; }
        string filePath { get; set; }
        Bitmap bitmap { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            ModeSelect.SelectedIndex = 0;
        }

        private void DisplayImage(string filePath)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath, UriKind.Absolute);
                bitmap.EndInit();
            }
            catch (Exception e)
            { MessageBox.Show($"Failed"); }
        
        }

        //connect to the funcion of saving the image to the provided filepath

        //ToDo: fix the logic of save button when Ui is done.
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            ImageHandler.SaveImage(filePath,bitmap);
        }

        //clearing the content within the image box
        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        
        private void ModeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear the dependent ComboBox
            OptionSelect.Items.Clear();
            OptionSelect.Items.Add(new ComboBoxItem { Content = "*", IsEnabled = false });
            ParamArea.Children.Clear();

            // Get the selected item from ModeSelect ComboBox
            string selectedOption = (ModeSelect.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Populate the OptionSelect ComboBox based on the ModeSelect choice
            if (selectedOption == "Embed")
            {
                TextBox inputTextBox = new TextBox
                {
                    Width = 200,
                    Height = 30,
                    Margin = new Thickness(0, 10, 0, 0),
                    Name = "UserInputTextBox",
                    Text = "Enter text for embedding..."
                };
                OptionSelect.Items.Add(new ComboBoxItem { Content = "DCT" });
                OptionSelect.Items.Add(new ComboBoxItem { Content = "LSBW" });
            }
            else if (selectedOption == "Attack")
            {
                OptionSelect.Items.Add(new ComboBoxItem { Content = "Rotation" });
                OptionSelect.Items.Add(new ComboBoxItem { Content = "Blurring" });
                OptionSelect.Items.Add(new ComboBoxItem { Content = "Cropping" });
                OptionSelect.Items.Add(new ComboBoxItem { Content = "Compression" });
            }
            else if (selectedOption == "Evaluate")
            {
                OptionSelect.Items.Add(new ComboBoxItem { Content = "Check" });
                OptionSelect.Items.Add(new ComboBoxItem { Content = "Identify" });
            }

            // Optionally set the first item as selected in OptionSelect
            if (OptionSelect.Items.Count > 0)
            {
                OptionSelect.SelectedIndex = 0; // Automatically select the first item
            }
        }

        List<string> labels_ParamArea;

        private void AppendTextblockToParamArea(Grid ParamGrid)
        {
            for (int i = 0; i < labels_ParamArea.Count(); i++)
            { ParamGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); }

            for (int i = 0; i < labels_ParamArea.Count(); i++)
            {
                TextBlock label = new TextBlock
                {
                    Text = labels_ParamArea[i],
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5)
                };
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, i);
                ParamGrid.Children.Add(label);

                TextBox paramTextBox = new TextBox
                {
                    Width = 200,
                    Height = 30,
                    Margin = new Thickness(0, 5, 0, 0),
                    Name = $"ParamTextBox{i}",
                    Text = $"Enter value {i}..."
                };
                Grid.SetColumn(paramTextBox, 1);
                Grid.SetRow(paramTextBox, i);
                // Add the TextBox to the ParamArea StackPanel
                ParamGrid.Children.Add(paramTextBox);
            }
            ParamArea.Children.Add(ParamGrid);
        }

        //Todo: Extrac out the append into a method.
        private void OptionSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string OperationSelected = (OptionSelect.SelectedItem as ComboBoxItem)?.Content.ToString();
            ParamArea.Children.Clear();

            Grid ParamGrid = new Grid { Margin = new Thickness(0, 5, 0, 0) };
            ParamGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            ParamGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });

            
            switch (OperationSelected)
            {
                case "DCT":
                    labels_ParamArea = ["Block Size","Embedding Strength","Quantization Factor","DCT Coeffient"];
                    AppendTextblockToParamArea(ParamGrid);
                    /*
                    for (int i = 0; i < labels_ParamArea.Count(); i++)
                    { ParamGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); }

                    for (int i = 0; i < labels_ParamArea.Count(); i++)
                    {
                        TextBlock label = new TextBlock
                        {
                            Text = labels_ParamArea[i],
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(5)
                        };
                        Grid.SetColumn(label, 0);
                        Grid.SetRow(label, i);
                        ParamGrid.Children.Add(label);

                        TextBox paramTextBox = new TextBox
                        {
                            Width = 200,
                            Height = 30,
                            Margin = new Thickness(0, 5, 0, 0),
                            Name = $"ParamTextBox{i}",
                            Text = $"Enter value {i}..."
                        };
                        Grid.SetColumn(paramTextBox, 1);
                        Grid.SetRow(paramTextBox, i);
                        // Add the TextBox to the ParamArea StackPanel
                        ParamGrid.Children.Add(paramTextBox);
                    }
                    ParamArea.Children.Add(ParamGrid);
                    */
                    return;
                case "LSBW":
                    labels_ParamArea = ["BitPlane Depth", "Channel selection", "Pixel Selection Method"];
                    AppendTextblockToParamArea(ParamGrid);
                    return;
                case "Rotation":
                    labels_ParamArea = ["Rotation Angle", "Rotation Direction", "Interpoloation method"];
                    AppendTextblockToParamArea(ParamGrid);
                    return;
                case "Blurring":
                    labels_ParamArea = ["Blur Type", "Kernel Size", "Blur Intensity"];
                    AppendTextblockToParamArea(ParamGrid);
                    return;
                case "Cropping":
                    labels_ParamArea = ["X Start Pos", "Y start Pos", "Width", "Height"];
                    AppendTextblockToParamArea(ParamGrid);
                    return;
                case "Compression":
                    labels_ParamArea = ["Compression Type", "Compression Format", "Compression Quality"];
                    AppendTextblockToParamArea(ParamGrid);
                    return;
                case "Check":
                    labels_ParamArea = ["Scanned Block", "Original Message"];
                    AppendTextblockToParamArea(ParamGrid);
                    return;
                case "Identify":
                    labels_ParamArea = ["Scanned Block", "Original Message", "If Corrupted"];
                    AppendTextblockToParamArea(ParamGrid);
                    return;
            }

            
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}