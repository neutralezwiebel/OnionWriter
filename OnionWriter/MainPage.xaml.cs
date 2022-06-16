﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage;
using Windows.UI.Text;
using Windows.UI.Popups;
using Windows.UI.Xaml.Documents;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Xml.Linq;
using System.Text;
using System.Xml;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Hosting;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Core;
using Windows.UI.Xaml.Printing;
using Windows.Graphics.Printing;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using Microsoft.Win32;
using Windows.ApplicationModel.Activation;

namespace Test
{

    public partial class Page : Windows.UI.Xaml.Controls.Page
    {
        //Variables
        #region
        public bool italic1 = false;
        public bool bold1 = false;
        public bool underlined1 = false;
        public int SelectionStart;
        public int SelectionEnd;
        public double Slidervalue;
        public Windows.UI.Xaml.Media.Brush Lightmodebrush1 = new SolidColorBrush(Colors.White);
        public Windows.UI.Xaml.Media.Brush Lightmodebrush2 = new SolidColorBrush(Colors.Black);
        public Windows.UI.Xaml.Media.Brush Lightmodebrush3 = new SolidColorBrush(Colors.DarkGray);
        public bool LightAnable;
        #endregion

        public Page()
        {
            this.InitializeComponent();
            var fonts = Microsoft.Graphics.Canvas.Text.CanvasTextFormat.GetSystemFontFamilies();
            Fonts.ItemsSource = fonts;
            Fonts.SelectedIndex = 0;
            TxtBox.FontFamily = new Windows.UI.Xaml.Media.FontFamily(Fonts.SelectedItem.ToString());
            Task.Run(ThemeAutoChange);
        }

        //themes
        #region
        public async Task ThemeAutoChange()
        {
            try
            {
                object readValue;
                while (true)
                {
                    readValue = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", null);
                    if (readValue == null)
                    {
                        await Task.Delay(3000);
                        continue;
                    }

                    int lel = Int32.Parse(readValue.ToString());
                    if (lel == 0)
                    {
                        LightAnable = true;
                        TxtBox.Foreground = Lightmodebrush2;
                        TxtBox.Background = Lightmodebrush3;
                        GroundGrid.Background = Lightmodebrush2;
                        StackOne.Background = Lightmodebrush1;
                        StackTwo.Background = Lightmodebrush1;
                        FileButton.Foreground = Lightmodebrush2;
                        EditButton.Foreground = Lightmodebrush2;
                        ItalicButton.Foreground = Lightmodebrush2;
                        BoldButton.Foreground = Lightmodebrush2;
                        Mitwirkende.Foreground = Lightmodebrush2;
                        Mitwirkende.Background = Lightmodebrush3;
                        TxtBox.BorderBrush = new SolidColorBrush(Colors.Blue);
                    }
                    else
                    {
                        LightAnable = false;
                        TxtBox.Foreground = Lightmodebrush1;
                        TxtBox.Background = Lightmodebrush3;
                        GroundGrid.Background = Lightmodebrush1;
                        StackOne.Background = Lightmodebrush2;
                        StackTwo.Background = Lightmodebrush2;
                        FileButton.Foreground = Lightmodebrush1;
                        EditButton.Foreground = Lightmodebrush1;
                        ItalicButton.Foreground = Lightmodebrush1;
                        BoldButton.Foreground = Lightmodebrush1;
                        Mitwirkende.Foreground = Lightmodebrush1;
                        Mitwirkende.Background = Lightmodebrush3;
                        TxtBox.BorderBrush = new SolidColorBrush(Colors.Gray);
                    }

                    await Task.Delay(3000);
                }
            }
            catch(Exception ex)
            {
                ContentDialog contentDialog = new ContentDialog();
                contentDialog.Title = "Couldn´t run the application";
                string lel = "exception";
                lel += ex.ToString();
                ((ContentControl)contentDialog).Content = lel;
                contentDialog.PrimaryButtonText = "OK";
                ContentDialog errorDialog = contentDialog;
                ContentDialogResult contentDialogResult = await errorDialog.ShowAsync();
                errorDialog = (ContentDialog)null;
            }
        }
        #endregion

        //open/save/new file/DragnDrop
        #region

        public async Task OpenFile (IStorageFile file)
        {
            if (file != null)
            {
                string filepath = file.Path.ToString();
                if (!filepath.EndsWith(".odt"))
                    ;
                Debug.WriteLine(filepath);
                filepath = (string)null;
            }
            try
            {
                IBuffer buffer = await FileIO.ReadBufferAsync((IStorageFile)file);
                DataReader reader = DataReader.FromBuffer(buffer);
                reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                string text = reader.ReadString(buffer.Length);
                this.TxtBox.Document.SetText((TextSetOptions)8192, text);
                buffer = (IBuffer)null;
                reader = (DataReader)null;
                text = (string)null;
            }
            catch (Exception ex)
            {
                ContentDialog contentDialog = new ContentDialog();
                contentDialog.Title = "File open error";
                ((ContentControl)contentDialog).Content = "Sorry, couldn't open the file.";
                contentDialog.PrimaryButtonText = "OK";
                ContentDialog errorDialog = contentDialog;
                ContentDialogResult contentDialogResult = await errorDialog.ShowAsync();
                errorDialog = (ContentDialog)null;
            }
            
            file = (StorageFile)null;
        }
        private async void OpennFile_Click(object sender, RoutedEventArgs e) => await this.OpenFileClickTask();
        private async Task OpenFileClickTask()
        {
            FileOpenPicker open = new FileOpenPicker();
            open.ViewMode = PickerViewMode.Thumbnail;
            open.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            open.FileTypeFilter.Add(".rtf");
            open.FileTypeFilter.Add(".txt");
            open.FileTypeFilter.Add(".docx");
            IStorageFile file = await open.PickSingleFileAsync();
            await OpenFile(file);
            open = (FileOpenPicker)null;
        }

        private async void SaveFile_Click(object sender, RoutedEventArgs e) => await this.SaveFileClickTask();
        private async Task SaveFileClickTask()
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Rich Text", (IList<string>)new List<string>() { ".rtf" });
            savePicker.FileTypeChoices.Add("Text", (IList<string>)new List<string>() { ".txt" });
            savePicker.SuggestedFileName = "New document";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file == null)
            {
                savePicker = (FileSavePicker)null;
                file = (StorageFile)null;
            }
            else
            {
                CachedFileManager.DeferUpdates((IStorageFile)file);
                using (IRandomAccessStream randAccStream = await file.OpenAsync((FileAccessMode)1))
                    this.TxtBox.Document.SaveToStream((TextGetOptions)8192, randAccStream);
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync((IStorageFile)file);
                if (status != FileUpdateStatus.Complete)
                {
                    MessageDialog errorBox = new MessageDialog("File " + file.Name + " couldn't be saved.");
                    IUICommand iuiCommand = await errorBox.ShowAsync();
                    errorBox = (MessageDialog)null;
                }
                savePicker = (FileSavePicker)null;
                file = (StorageFile)null;
            }
        }



        private void TxtBox_DragOver(object sender, DragEventArgs e)
        {
            e.DragUIOverride.Caption = "drop here";
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = false;
        }

        private async void TxtBox_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    IReadOnlyList<IStorageItem> items = await e.DataView.GetStorageItemsAsync();
                    if (((IReadOnlyCollection<IStorageItem>)items).Count > 0)
                    {
                        if (!this.LightAnable)
                            ((Control)this.TxtBox).Foreground = Lightmodebrush1;
                        else
                            ((Control)this.TxtBox).Foreground = Lightmodebrush2;
                        StorageFile storageFile = items[0] as StorageFile;
                        using (await storageFile.OpenAsync((FileAccessMode)0))
                        {
                            IBuffer buffer = await FileIO.ReadBufferAsync((IStorageFile)storageFile);
                            DataReader reader = DataReader.FromBuffer(buffer);
                            reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                            string text = reader.ReadString(buffer.Length);
                            this.TxtBox.Document.SetText((TextSetOptions)8192, text);
                            buffer = (IBuffer)null;
                            reader = (DataReader)null;
                            text = (string)null;
                        }
                        storageFile = (StorageFile)null;
                    }
                    items = (IReadOnlyList<IStorageItem>)null;
                }
            }
            catch (Exception ex)
            {
                ContentDialog contentDialog = new ContentDialog();
                contentDialog.Title = "Open file error";
                ((ContentControl)contentDialog).Content = "Sorry, couldn't open the file.";
                contentDialog.PrimaryButtonText = "OK";
                ContentDialog ErrorDialog = contentDialog;
                ContentDialogResult contentDialogResult = await ErrorDialog.ShowAsync();
                ErrorDialog = (ContentDialog)null;
            }
        }


        private async void New_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush standartbrush = new SolidColorBrush(Colors.DarkGray);
            SolidColorBrush standartbrushbk = new SolidColorBrush(Colors.Black);
            ContentDialog contentDialog = new ContentDialog();
            contentDialog.Title = "Create new file";
            ((ContentControl)contentDialog).Content = "Do you want to save the changes?";
            contentDialog.PrimaryButtonText = "Save";
            contentDialog.SecondaryButtonText = "Delete";
            contentDialog.CloseButtonText = "Cancel";
            ContentDialog newFileDialog = contentDialog;
            string Text = null;
            TxtBox.Document.GetText((TextGetOptions)0, out Text);
            if (Text.Length - 1 <= 0)
            {
                standartbrush = (SolidColorBrush)null;
                standartbrushbk = (SolidColorBrush)null;
                newFileDialog = (ContentDialog)null;
                Text = (string)null;
            }
            else
            {
                ContentDialogResult result = await newFileDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                    await this.SaveFileClickTask();
                else if (result == ContentDialogResult.Secondary)
                {
                    this.TxtBox.Document.SetText((TextSetOptions)0, string.Empty);
                    ((Control)this.TxtBox).Foreground = standartbrushbk;
                    ((Control)this.TxtBox).Background = standartbrush;
                }
                standartbrush = (SolidColorBrush)null;
                standartbrushbk = (SolidColorBrush)null;
                newFileDialog = (ContentDialog)null;
                Text = (string)null;
            }
        }
        #endregion

        //print
        #region

        private PrintManager printMan;
        private PrintDocument printDoc;
        private IPrintDocumentSource printDocSource;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var b = e.Parameter as Windows.ApplicationModel.Activation.IActivatedEventArgs;
            if (b != null)
            {
                FileActivatedEventArgs faileInfo = (FileActivatedEventArgs)b;
                OpenAssociadetFile(faileInfo);
            }
            // Register for PrintTaskRequested event
            printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += PrintTaskRequested;
            // Build a PrintDocument and register for callbacks
            printDoc = new PrintDocument();
            printDocSource = printDoc.DocumentSource;
            printDoc.Paginate += Paginate;
            printDoc.GetPreviewPage += GetPreviewPage;
            printDoc.AddPages += AddPages;
        }
        private async void Print_Click(object sender, RoutedEventArgs e)
        {
            var Lightmodebrush = new SolidColorBrush(Colors.White);
            TxtBox.Background = Lightmodebrush;
            if (PrintManager.IsSupported())
            {
                try
                {
                    // Show print UI
                    await PrintManager.ShowPrintUIAsync();
                }
                catch
                {
                    // Printing cannot proceed at this time
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, printing can' t proceed at this time.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                }
            }
            else
            {
                // Printing is not supported on this device
                ContentDialog noPrintingDialog = new ContentDialog()
                {
                    Title = "Printing not supported",
                    Content = "\nSorry, printing is not supported on this device.",
                    PrimaryButtonText = "OK"
                };
                await noPrintingDialog.ShowAsync();
            }
        }

        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            // Create the PrintTask.
            // Defines the title and delegate for PrintTaskSourceRequested
            var printTask = args.Request.CreatePrintTask("Print", PrintTaskSourceRequrested);

            // Handle PrintTask.Completed to catch failed print jobs
            printTask.Completed += PrintTaskCompleted;
        }
        private void PrintTaskSourceRequrested(PrintTaskSourceRequestedArgs args)
        {
            // Set the document source.
            args.SetSource(printDocSource);
        }
        private void Paginate(object sender, PaginateEventArgs e)
        {
            // As I only want to print one Rectangle, so I set the count to 1
            printDoc.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }

        private void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            // Provide a UIElement as the print preview.
            printDoc.SetPreviewPage(e.PageNumber, this.TxtBox);
        }
        private void AddPages(object sender, AddPagesEventArgs e)
        {
            printDoc.AddPage(this.TxtBox);

            // Indicate that all of the print pages have been provided
            printDoc.AddPagesComplete();
            var Lightmodebrush1 = new SolidColorBrush(Colors.DarkGray);
            TxtBox.Background = Lightmodebrush1;
        }
        private async void PrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
        {

            // Notify the user when the print operation fails.
            if (args.Completion == PrintTaskCompletion.Failed)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {

                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, failed to print.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();

                });

            }

        }
        #endregion
        //Text Edit
        #region
        private void Fonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }
            TxtBox.FontFamily = new Windows.UI.Xaml.Media.FontFamily(e.AddedItems[0].ToString());

        }
        public void txtgrosse_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            float b1 = (float)txtgrosse.Value;
            ITextRange Range = TxtBox.Document.Selection;
            int lo = TxtBox.Document.Selection.Length;
            if (lo == 0)
            {
                TxtBox.FontSize = txtgrosse.Value;
            }
            else
            {
                Windows.UI.Text.ITextCharacterFormat charFormatting = Range.CharacterFormat;
                charFormatting.Size = b1;
            }
        }
        private void Italic_Click(object sender, RoutedEventArgs e)
        {
            if (italic1 == false)
            {
                TxtBox.FontStyle = Windows.UI.Text.FontStyle.Italic;
                italic1 = true;
            }
            else
            {
                TxtBox.FontStyle = Windows.UI.Text.FontStyle.Normal;
                italic1 = false;
            }
        }

        private void Bold_Click(object sender, RoutedEventArgs e)
        {
            if (bold1 == false)
            {
                TxtBox.FontWeight = FontWeights.Bold;
                bold1 = true;
            }
            else
            {
                TxtBox.FontWeight = FontWeights.Normal;
                bold1 = false;
            }
        }
        private async void color_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ContentDialog();
            dlg.Content = new ColorPicker();
            dlg.PrimaryButtonText = "okay";
            dlg.SecondaryButtonText = "Cancel";
            await dlg.ShowAsync();
            Windows.UI.Text.ITextSelection selection = TxtBox.Document.Selection;
            if (selection.Length < 0)
            {

                ITextCharacterFormat charFormatting = selection.CharacterFormat;

                charFormatting.ForegroundColor = ((ColorPicker)dlg.Content).Color;

                selection.CharacterFormat = charFormatting;
            }
            else
            {
                var colorb = new SolidColorBrush(((ColorPicker)dlg.Content).Color);
                TxtBox.Foreground = colorb;
            }

        }
        #endregion
        private async void Mitwirkende_Click(object sender, RoutedEventArgs e)
        {
            var currentAV = ApplicationView.GetForCurrentView();
            var newAV = CoreApplication.CreateNewView();
            await newAV.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal,
                            async () =>
                            {
                                var newWindow = Window.Current;
                                var newAppView = ApplicationView.GetForCurrentView();
                                newAppView.Title = "New window";
                                newAppView.SetPreferredMinSize(new Windows.Foundation.Size(1500, 825));
                                newAppView.VisibleBoundsChanged += (o, args) => newAppView.TryResizeView(new Windows.Foundation.Size(1500, 825));


                                var frame = new Frame();
                                frame.Navigate(typeof(BlankPage1), null);
                                newWindow.Content = frame;
                                newWindow.Activate();

                                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(
                                    newAppView.Id,
                                    ViewSizePreference.Default,
                                    currentAV.Id,
                                    ViewSizePreference.Default);
                            });
        }
            
           
            
        public async void OpenAssociadetFile(FileActivatedEventArgs file)
        {
            IStorageItem[] paths = file.Files.Where(i => i != null && (Path.GetExtension(i.Path) == ".rtf" || Path.GetExtension(i.Path) == ".txt")).ToArray();
            if (paths.Length == 1)
            {
               
                await OpenFile((IStorageFile)paths[0]);
            }
            else
            {

            }
 
        }

    }
}