using System.Drawing;
using System.IO;
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
using static System.Net.Mime.MediaTypeNames;

namespace Testing_WPF
{

    //ToDo: add more guidance
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ImageHandling ImageHandler = new ImageHandling();
        string filePath;
        Bitmap bitmap;
        int blocksize = 8;
        Matrix A, R, G, B;
        Block[,] blocks;
        Watermark wm;

        List<string> labels_ParamArea;

        public MainWindow()
        {
            InitializeComponent();
            ModeSelect.SelectedIndex = 0;
        }

        private void DisplayImage(string filePath)
        {
            try
            {
                bitmap = new Bitmap(filePath);
                bitmap = ImageHandler.TrimToBlockSize(bitmap, blocksize);
                MainPictureDisplay.Source = new BitmapImage(new Uri(filePath));
                (A,R,G,B) = ImageHandler.ConvertImageToMatrix(bitmap);
                blocks = ImageHandler.SplitToBlocks(A, R, G, B, blocksize);
            }
            catch (Exception e)
            { MessageBox.Show($"Failed"); }
        }
        private BitmapImage DisplayImage(Bitmap bmp)
        {
            BitmapImage image = new BitmapImage();
            try
            {
                using (MemoryStream memory = new MemoryStream())
                { bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = memory;
                    image.EndInit();
                }
            }
            catch (Exception e)
            { MessageBox.Show($"Failed"); }
            return image;
        }



        //clearing the content within the image box
        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        { MainPictureDisplay.Source = null; }

        
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

        string OperationSelected = "";
        private void OptionSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OperationSelected = (OptionSelect.SelectedItem as ComboBoxItem)?.Content.ToString();
            ParamArea.Children.Clear();

            Grid ParamGrid = new Grid { Margin = new Thickness(0, 5, 0, 0) };
            ParamGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            ParamGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            
            switch (OperationSelected)
            {
                case "DCT":
                    labels_ParamArea = ["Message", "DCT Coeffient Row", "DCT Coeffient Column", "Embedding Strength"];
                    AppendTextblockToParamArea(ParamGrid); 
                    return;
                case "LSBW":
                    labels_ParamArea = ["Message", "Channel selection"];
                    AppendTextblockToParamArea(ParamGrid);
                    return;
                case "Rotation":
                    labels_ParamArea = ["Rotation Angle"];
                    AppendTextblockToParamArea(ParamGrid);
                    return;
                case "Blurring":
                    labels_ParamArea = ["Kernel Size", "Blur Intensity"];
                    AppendTextblockToParamArea(ParamGrid);
                    return;
                case "Cropping":
                    labels_ParamArea = ["Crop Ratio"];
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
            //Read all inputted values in ParamArea
            List<string> paramValues = new List<string>();
            foreach (var container in ParamArea.Children)
            {
                if (container is Grid grid)
                {
                    foreach (var child in grid.Children)
                    {
                        if (child is TextBox tb)
                        {
                            string value = tb.Text;
                            paramValues.Add(value);
                        }
                    }
                }
                
            }
            switch (OperationSelected)
            {
                case "DCT":
                    wm = new Watermark(paramValues[0]);//new Watermark("where was the tea my friend where was the tea my");//placeholder
                    Embedder DCTEmbedder = new DCTEmbedder(Convert.ToInt32(paramValues[1]), Convert.ToInt32(paramValues[2]), Convert.ToInt32(paramValues[3]));
                    DCTEmbedder.SetBlockGrid(blocks);
                    DCTEmbedder.EmbedWatermark(wm);
                    Block[,] DCTmodifiedBlocks = DCTEmbedder.GetBlockGrid();
                    bitmap = ImageHandler.MergeBlocksToImage(DCTmodifiedBlocks);
                    DisplayImage(bitmap);
                    MessageBox.Show($"Acomplished");
                    break;
                case "LSBW":
                    wm = new Watermark(paramValues[0]);//placeholder
                    Embedder LSBWEmbedder = new LSBWEmbedder(paramValues[1]);
                    LSBWEmbedder.SetBlockGrid(blocks);
                    LSBWEmbedder.EmbedWatermark(wm);
                    Block[,] LSBWmodifiedBlocks = LSBWEmbedder.GetBlockGrid();
                    bitmap = ImageHandler.MergeBlocksToImage(LSBWmodifiedBlocks);
                    DisplayImage(bitmap);
                    MessageBox.Show($"Acomplished");
                    break;
                case "Rotation":
                    Attack rot_atk = new ImageRotation(float.Parse(paramValues[0]));
                    bitmap = rot_atk.ApplyAttack(bitmap);
                    DisplayImage(bitmap);
                    MessageBox.Show($"Acomplished");
                    break;
                case "Blurring":
                    Attack blur_atk = new ImageBlurring(Convert.ToInt32(paramValues[0]), Convert.ToDouble(paramValues[1]));
                    bitmap = blur_atk.ApplyAttack(bitmap);
                    DisplayImage(bitmap);
                    MessageBox.Show($"Acomplished");
                    return;
                case "Cropping":
                    Attack crop_atk = new ImageRotation(float.Parse(paramValues[0]));
                    bitmap = crop_atk.ApplyAttack(bitmap);
                    DisplayImage(bitmap);
                    MessageBox.Show($"Acomplished");
                    return;
                case "Compression":
                    Attack comp_atk = new ImageRotation(float.Parse(paramValues[0]));
                    bitmap = comp_atk.ApplyAttack(bitmap);
                    DisplayImage(bitmap);
                    MessageBox.Show($"Acomplished");
                    return;
                case "Check":
                    
                    return;
                case "Identify":
                    
                    return;
            }

        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            filePath = FilePath_TextBox.Text;
            DisplayImage(filePath);
        }

        
        
    }
}