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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string filePath = @"C:\Users\YANGAN\OneDrive - The Kings School Chester\unopics\B0.png";
            DisplayImage(filePath);
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
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {

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

            // Get the selected item from ModeSelect ComboBox
            string selectedOption = (ModeSelect.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Populate the OptionSelect ComboBox based on the ModeSelect choice
            if (selectedOption == "Embed")
            {
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

        private void OptionSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string OperationSelected = (OptionSelect.SelectedItem as ComboBoxItem)?.Content.ToString();
            switch (OperationSelected)
            {
                case "DCT":

                    return;
                case "LSBW":
                    return;
                case "Rotation":
                    return;
                case "Blurring":
                    return;
                case "Cropping":
                    return;
                case "Compression":
                    return;
                case "Check":
                    return;
                case "Identify":
                    return;
            }

            
        }
    }
}