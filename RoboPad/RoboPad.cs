using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gdk;
using Gtk;
using GtkSource;
using File = System.IO.File;
using MenuItem = Gtk.MenuItem;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;
using WrapMode = Gtk.WrapMode;

namespace RoboPad
{
    public class RoboPad : Window
    {
        private static int noOfWindows = 0;
        private SourceView sourceView;

        private string activeFile = "";
        private string activeText = "";
        private HeaderBar headerBar;
        private String headerBarOringalText = "";
        private bool isChangeSaved = true;
        private String previousText = "";
        private Label statusBarLabel;
        private LanguageSelectorButton languageSelectorButton;
        // private MenuButton languageSelectorButton;

        
        public RoboPad(WindowType type, String[] args) : base(type)
        {

            noOfWindows++;
            SetDefaultSize(500, 500);
            Maximize();
            DeleteEvent += (sender, s) =>
            {
                if (!isChangeSaved)
                {
                    MessageDialog dialog = new MessageDialog(this, DialogFlags.DestroyWithParent, 
                        MessageType.Warning, ButtonsType.OkCancel, "Changes Unsaved Are You Sure You Want To Exit ");
                    // Dialog dialog=new Dialog()
                    var response = dialog.Run();
                    noOfWindows--;
                    if (response == (int) Gtk.ResponseType.Ok)
                    {
                        if (noOfWindows <= 0)
                        {
                            Gtk.Application.Quit();
                        }
                    }
                    else
                    {
                        s.RetVal = true;
                    }
                    dialog.Dispose();
                }
                else
                {
                    noOfWindows--;
                    if (noOfWindows <= 0)
                    {
                        Gtk.Application.Quit();
                    }
                }
            };
            VBox vBox = new VBox();

            List<String> newTextList = new();
            var lm= LanguageManager.Default;

            sourceView = new();
            sourceView.Buffer.HighlightSyntax = true;
            sourceView.WrapMode = WrapMode.Word;
            sourceView.AutoIndent = true;
            sourceView.ShowLineNumbers = true;
            StyleSchemeManager schemeManager=StyleSchemeManager.Default;
            // StyleScheme styleScheme = schemeManager.GetScheme("pop-light");

            // sourceView.Buffer.StyleScheme = styleScheme;
            
            sourceView.KeyPressEvent += (o, eventArgs) =>
            {
                if (Keyval.Name(eventArgs.Event.KeyValue) == "s")
                {
                    saveFile(o,eventArgs);
                }
                else if (Keyval.Name(eventArgs.Event.KeyValue) == "S")
                {
                    saveAsFile(o,eventArgs);
                }
                else if (Keyval.Name(eventArgs.Event.KeyValue) == "o")
                {
                    openFile(o,eventArgs);   
                }
            };
            
            
            headerBar = new HeaderBar();
            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.Add(sourceView);

            headerBar.ShowCloseButton = true;
            headerBar.Title = "RoboPad";
            headerBarOringalText = headerBar.Title;
            
            sourceView.Buffer.Changed += (sender, eventArgs) =>
            {
                headerBar.Title = "*" + headerBarOringalText;
                if (sourceView.Buffer.Text != previousText)
                    isChangeSaved = false;
                else
                {
                    isChangeSaved = true;
                    headerBar.Title = headerBarOringalText;
                }
            };
            ToolButtonHandler toolbar = new ToolButtonHandler(this,sourceView);

            
            
            // Menu menu = new Menu();
            // MenuItem openItem = new RadioMenuItem("test");
            // menu.Add(openItem);
            //
            // menu.ShowAll();
            //

 


            PopOverMenuHandler popoverMenu = new(this);
            MenuButton menuButton = new MenuButton();
            menuButton.Popover = popoverMenu;
            Image image= Gtk.Image.NewFromIconName("view-more-symbolic",IconSize.LargeToolbar);
            menuButton.Image = image;
            headerBar.PackEnd(menuButton);

            Statusbar statusbar = new Statusbar();
            statusBarLabel = new("");
            statusbar.PackStart(statusBarLabel,false,false,5);
            languageSelectorButton = new LanguageSelectorButton(sourceView);
            statusbar.PackEnd(languageSelectorButton,false,false,5);






            // Box box = new Box(Gtk.Orientation.Vertical,2);

            Titlebar = headerBar;

            vBox.PackStart(toolbar,false,false,0);
            vBox.PackStart(scrolledWindow,true,true,0);
            vBox.PackStart(statusbar,false,false,0);
            Add(vBox);
            
            ShowAll();
            
            Show();
            if (args!=null)
            {
                if (args.Length >= 1)
                {
                    if (!String.IsNullOrWhiteSpace(args[0]))
                    {
                        openFileWithArgs(args[0]);
                    
                    }
                }
            }
        }

        
        public void saveFile(object sender,EventArgs args)
        {
            FileChooserDialog filesaveDialog = new FileChooserDialog("Save File",this,FileChooserAction.Save);
            filesaveDialog.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel );
            filesaveDialog.AddButton(Gtk.Stock.Save, Gtk.ResponseType.Ok);
            if (!File.Exists(activeFile))
            {
                var respone = filesaveDialog.Run();
                if (respone == (int) Gtk.ResponseType.Ok)
                {
                    activeFile = filesaveDialog.Filename;
                    string currentText = sourceView.Buffer.Text;
                    var lm = LanguageManager.Default;
                    var lang = lm.GuessLanguage(activeFile,"");
                    sourceView.Buffer.Language = lang;
                    if (lang != null)
                    {
                        languageSelectorButton.Label = lang.Name;
                    }
                    else
                    {
                        languageSelectorButton.Label = "Text";
                    }

                    statusBarLabel.Text = "Saved File";
                    previousText = currentText;
                    headerBar.Subtitle = activeFile;
                    headerBar.Title = headerBarOringalText;
                    isChangeSaved = true;

                    // StreamWriter writer = new StreamWriter(activeFile, false);
                    // writer.Write(currentText);
                    using (var stream = File.Create(activeFile))
                    {
                        using (TextWriter tw=new StreamWriter(stream))
                        {
                            tw.Write(currentText);
                        }
                    }
                    File.WriteAllText(activeFile,currentText);
                    // Console.WriteLine("ok");
                }
                // else if (respone == (int) Gtk.ResponseType.Cancel)
                // {
                //     Console.WriteLine("cancel");
                // }

                filesaveDialog.Dispose();
            }
            else
            {
                headerBar.Subtitle = activeFile;
                headerBar.Title = headerBarOringalText;
                isChangeSaved = true;
                string currentText = sourceView.Buffer.Text;
                previousText = currentText;
                File.WriteAllText(activeFile,currentText);

            }
        }
        
        public void saveAsFile(object sender,EventArgs args)
        {
            FileChooserDialog filesaveDialog = new FileChooserDialog("Save File",this,FileChooserAction.Save);
            filesaveDialog.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel );
            filesaveDialog.AddButton(Gtk.Stock.Save, Gtk.ResponseType.Ok);
            
                var respone = filesaveDialog.Run();
                if (respone == (int) Gtk.ResponseType.Ok)
                {
                    activeFile = filesaveDialog.Filename;
                    string currentText = sourceView.Buffer.Text;
                    var lm = LanguageManager.Default;
                    var lang = lm.GuessLanguage(activeFile,"");
                    sourceView.Buffer.Language = lang;
                    if (lang != null)
                    {
                        languageSelectorButton.Label = lang.Name;
                    }
                    else
                    {
                        languageSelectorButton.Label = "Text";
                    }
                    statusBarLabel.Text = "Saved File";
                    previousText = currentText;
                    headerBar.Subtitle = activeFile;
                    headerBar.Title = headerBarOringalText;
                    isChangeSaved = true;


                    // StreamWriter writer = new StreamWriter(activeFile, false);
                    // writer.Write(currentText);
                    using (var stream = File.Create(activeFile))
                    {
                        using (TextWriter tw=new StreamWriter(stream))
                        {
                            tw.Write(currentText);
                        }
                        
                    }
                    // Console.WriteLine("ok");
                }
                // else if (respone == (int) Gtk.ResponseType.Cancel)
                // {
                //     Console.WriteLine("cancel");
                //     
                // }
                filesaveDialog.Dispose();
        }
        public void openFile(object sender, EventArgs args)
        {
            String text;
            FileChooserDialog fileopenDialog = new FileChooserDialog("Open File",this,FileChooserAction.Open);
            fileopenDialog.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel );
            fileopenDialog.AddButton(Gtk.Stock.Open, Gtk.ResponseType.Ok);
            var respone= fileopenDialog.Run();
            if (respone == (int)Gtk.ResponseType.Ok)
            {
                activeFile = fileopenDialog.Filename;
                // Console.WriteLine(activeFile);
                headerBar.Subtitle = activeFile;
                text = File.ReadAllText(activeFile);
                var lm = LanguageManager.Default;
                var lang = lm.GuessLanguage(activeFile,"");
                // if(lang!=null) Console.WriteLine(lang.Name);
                sourceView.Buffer.Language = lang;
                if (lang != null)
                {
                    languageSelectorButton.Label = lang.Name;
                }
                else
                {
                    languageSelectorButton.Label = "Text";
                }
                statusBarLabel.Text = "Opened File";

                sourceView.Buffer.Text = text;
                headerBar.Title = headerBarOringalText;
                // defaultTextList = Regex.Split(textView.Buffer.Text, @"(\s+|\n+)").ToList();
                // defaultText = text;
                isChangeSaved = true;
                
            }
            // else if (respone == (int) Gtk.ResponseType.Cancel)
            // {
            //     
            // }
            fileopenDialog.Dispose();


        }

        public void openFileWithArgs(String arg)
        {
            String text;
            if(File.Exists(arg))
            {
                activeFile = arg;
                activeFile = System.IO.Path.GetFullPath(activeFile);
                headerBar.Subtitle = activeFile;
                text = File.ReadAllText(activeFile);
                var lm = LanguageManager.Default;
                var lang = lm.GuessLanguage(activeFile,"");
                sourceView.Buffer.Language = lang;
                if (lang != null)
                {
                    languageSelectorButton.Label = lang.Name;
                }
                else
                {
                    languageSelectorButton.Label = "Text";
                }
                
                statusBarLabel.Text = "Opened File";
                sourceView.Buffer.Text = text;
                
            }
            else
            {
                
                activeFile = arg;
                activeFile = System.IO.Path.GetFullPath(activeFile);
                string currentText = "";
                using (var stream = File.Create(activeFile))
                {
                    using (TextWriter tw=new StreamWriter(stream))
                    {
                        tw.Write(currentText);
                    }
                }
                headerBar.Subtitle = activeFile;
                text = File.ReadAllText(activeFile);
                var lm = LanguageManager.Default;
                var lang = lm.GuessLanguage(activeFile,"");
                sourceView.Buffer.Language = lang;
                if (lang != null)
                {
                    languageSelectorButton.Label = lang.Name;
                }
                else
                {
                    languageSelectorButton.Label = "Text";
                }
                statusBarLabel.Text = "Opened File";
                // defaultTextList = Regex.Split(textView.Buffer.Text, @"(\s+|\n+)").ToList();
                sourceView.Buffer.Text = text;

            }
        }
        public static void Main(string[] args)
        {
            String str = "\n\n\n";
            
            Gtk.Application.Init();
            new RoboPad(WindowType.Toplevel,args);
            Gtk.Application.Run();

        }
    }
}
