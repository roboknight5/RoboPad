using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gdk;
using GLib;
using Gtk;
using GtkSource;
using Encoding = System.Text.Encoding;
using File = System.IO.File;
using Menu = Gtk.Menu;
using MenuItem = Gtk.MenuItem;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;
using WrapMode = Gtk.WrapMode;

namespace RoboPad
{
    class Program : Window
    {
        private static int noOfWindows = 0;
        private TextView textView;
        private SourceView sourceView;

        private string activeFile = "";
        private string activeText = "";
        private HeaderBar headerBar;
        private String headerBarOringalText = "";
        private bool isChangeSaved = true;
        private String previousText = "";
        private Label statusBarLabel;
        private MenuButton languageSelectorButton;

        private List<String> fullTextList;

        // private List<String> defaultTextList;
        private String defaultText;

        private Dictionary<string, string> languageMap = new()
        {
            {"txt","Text"},
            {"python3","Python 3"},
            {"python","Python"},
            {"css","CSS"},
            {"c","C"},
            {"cpp","C++"},
            {"java","Java"},
            {"js","JavaScript"},
            {"c-sharp","C#"}
        };
        



        public Program(WindowType type, String[] args) : base(type)
        {
            foreach (var  (key ,val) in languageMap)
            {
                // Console.WriteLine($"{key} {val}");
            }
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
            fullTextList = new();
            var lm= LanguageManager.Default;

            // Console.WriteLine(lm.LanguageIds);
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
            // sourceView.HighlightCurrentLine = true;
            
           

            headerBar = new HeaderBar();
            
            // textView = new TextView();
            // textView.WrapMode = WrapMode.Word;
            // textView.VscrollPolicy = ScrollablePolicy.Minimum;
            // textView.Buffer.UserActionEnded += (o, textArgs) =>
            // {
            //     fullTextList.Clear();
            //     l2.Clear();
            //     fullTextList= Regex.Split(textView.Buffer.Text, @"(\s+|\n+)").ToList();
            //     Console.Clear();
            //     // fullTextList.ForEach(s => { s.Replace("\n", "1");});
            //     // fullTextList= textView.Buffer.Text.Split().ToList();
            //     // tempTextList.ForEach(s =>
            //     // {
            //     //     if (s.Contains(Environment.NewLine))
            //     //     {
            //     //         var temp=s.Split(Environment.NewLine).ToList();
            //     //         // fullTextList.AddRange(temp);
            //     //     }
            //     //     else
            //     //     {
            //     //         fullTextList.Add(s);
            //     //     }
            //     // });
            //     fullTextList.ForEach(s => Console.Write($"{s}"));




            // fullTextList= textView.Buffer.Text.ToCharArray().Select(c => c.ToString()).ToArray().ToList();
            // fullTextList = splitText(textView.Buffer.Text);
            // fullTextList = Regex.Split(textView.Buffer.Text, @"(?<=[\\s])").ToList();

            // string text =textView.Buffer.Text;
            // fullTextList = Regex.Split(text,"\\s+").ToList();
        




        // textView.KeyPressEvent += (o, eventArgs) =>
        // {
        //     String undoText = "";
        //     // List<String> l2 = new List<string>();
        //     // Console.WriteLine(Keyval.Name(eventArgs.Event.KeyValue));
        //     // Console.WriteLine(eventArgs.Event.HardwareKeycode);
        //     if (Keyval.Name(eventArgs.Event.KeyValue) == "s")
        //     {
        //         saveFile(o, eventArgs);
        //     }
        //     else if (Keyval.Name(eventArgs.Event.KeyValue) == "z")
        //     {
        //         // defaultTextList.ForEach(Console.Write);
        //         // Console.WriteLine(!fullTextList.SequenceEqual(defaultTextList));
        //         // Console.Clear();
        //         // fullTextList.ForEach(s => Console.WriteLine($"elemnt : {s}"));
        //         if (fullTextList.Count != 0)
        //         {
        //             Console.Clear();
        //             Console.WriteLine("here");
        //             String item = "";
        //             fullTextList.ForEach(s => { item += s; });
        //             if (item != defaultText)
        //             {
        //                 l2.Add(fullTextList[fullTextList.Count - 1]);
        //                 fullTextList.RemoveAt(fullTextList.Count - 1);
        //             }
        //
        //             item = "";
        //             fullTextList.ForEach(s => { item += s; });
        //
        //
        //             // Console.WriteLine(item);
        //             textView.Buffer.Text = item;
        //         }
        //
        //
        //         // if (undoList.Count > 0)
        //         {
        //             // undoText = undoList.Pop();
        //             // redoList.Push(undoText);
        //             // undoList.ToList().ForEach(Console.WriteLine);
        //             // textView.Buffer.Text = undoText;
        //         }
        //
        //
        //         // Console.WriteLine(fullTextList.Count);
        //         // if (fullTextList.Count != 0)
        //         {
        //             // Console.WriteLine("here");
        //             // fullTextList.ForEach(Console.Write);
        //             // fullTextList.ForEach(Console.WriteLine);
        //
        //
        //             try
        //             {
        //                 // int rightPadding=fullTextList[fullTextList.Count].Length;
        //                 // int leftPadding = fullTextList[fullTextList.Count-2].Length;
        //                 // Console.WriteLine(fullTextList.Contains(Environment.NewLine));
        //                 // l2.Add(fullTextList[fullTextList.Count-1]);
        //                 // if (fullTextList[fullTextList.Count - 1] != "\n")
        //                 //     ;
        //                 // // l2.Add(" "+fullTextList[fullTextList.Count-1].PadRight(0).PadLeft(leftPadding));
        //                 // else
        //                 // {
        //                 //     l2.Add("\n");
        //                 // }
        //
        //                 // Console.WriteLine(l2[l2.Count-1]);
        //
        //             }
        //             catch (Exception e)
        //             {
        //             }
        //             // fullTextList.RemoveAt(fullTextList.Count-1);
        //             //  // String item = String.Join("",fullTextList);
        //             //  String item = "";
        //             //  foreach (var VARIABLE in fullTextList)
        //             //  {
        //             //      item+=VARIABLE;
        //             //      
        //             //  }
        //             //  // Console.Write(item);
        //             //  textView.Buffer.Text = item;
        //             //
        //
        //             // Console.WriteLine(item);
        //             foreach (var VARIABLE in fullTextList)
        //             {
        //                 // Console.WriteLine(VARIABLE);
        //
        //             }
        //
        //             foreach (var VARIABLE in l2)
        //             {
        //                 // Console.WriteLine(VARIABLE);
        //
        //             }
        //         }
        //
        //
        //
        //
        //     }
        //     else if (Keyval.Name(eventArgs.Event.KeyValue) == "Z")
        //     {
        //
        //
        //         // Console.WriteLine("test");
        //         if (l2.Count != 0)
        //         {
        //             // fullTextList=splitText(fullTextList);
        //
        //             // l2.ForEach(Console.Write);
        //             // fullTextList = splitText(fullTextList);
        //             String item = l2[l2.Count - 1];
        //             fullTextList.Add(item);
        //             l2.RemoveAt(l2.Count - 1);
        //             // l2.ForEach(s => Console.Write(s));
        //
        //             item = "";
        //             foreach (var VARIABLE in fullTextList)
        //             {
        //                 item += VARIABLE;
        //
        //             }
        //             textView.Buffer.Text = item;
        //         }
        //     }
        // };



    


    // l2.ForEach(Console.WriteLine);



                
            
            
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

            
            
            
            Toolbar toolbar = new Toolbar();
            toolbar.ToolbarStyle = ToolbarStyle.Icons;

            ToolButton newToolButton = new ToolButton("NewToolButton");
            newToolButton.IconName = "document-new";
            toolbar.Insert(newToolButton,0);
            newToolButton.TooltipText = "New Window";
            
            ToolButton openToolButton = new ToolButton("OpenToolButton");
            openToolButton.IconName = "document-open";
            toolbar.Insert(openToolButton,1);
            openToolButton.TooltipText = " Open";
            
            ToolButton saveToolButton = new ToolButton("SaveToolButton");
            saveToolButton.IconName = "document-save";
            toolbar.Insert(saveToolButton,2);
            saveToolButton.TooltipText = "Save";
            
            ToolButton saveAsToolButton = new ToolButton("SaveAsToolButton");
            saveAsToolButton.IconName = "document-save-as";
            saveAsToolButton.TooltipMarkup = "Save As";
            toolbar.Insert(saveAsToolButton,3);

            ToolButton undoToolButton = new ToolButton("UndoToolButton");
            undoToolButton.IconName = "edit-undo";
            undoToolButton.TooltipMarkup = "Undo Text";
            toolbar.Insert(undoToolButton,4);
            
            ToolButton redoToolButton = new ToolButton("RedoToolButton");
            redoToolButton.IconName = "edit-redo";
            redoToolButton.TooltipMarkup = "Redo Text";
            toolbar.Insert(redoToolButton,5);
            ;
            
            
            
            
            openToolButton.Clicked += openFile;
            saveToolButton.Clicked += saveFile;
            saveAsToolButton.Clicked += saveAsFile;
            newToolButton.Clicked += (Events, Args) =>
            {
                new Program(WindowType.Toplevel,null);
            };
            undoToolButton.Clicked += (sender, eventArgs) => sourceView.Buffer.Undo();
            redoToolButton.Clicked += (sender, eventArgs) => sourceView.Buffer.Redo();

            
            
            // Menu menu = new Menu();
            // MenuItem openItem = new RadioMenuItem("test");
            // menu.Add(openItem);
            //
            // menu.ShowAll();
            //

            PopoverMenu popoverMenu = new PopoverMenu();
            
            ModelButton newWindowButton = new ModelButton();
            newWindowButton.Text = "New Window";
            newWindowButton.Margin = 5;
            newWindowButton.SetSizeRequest(20,20);

            newWindowButton.Show();
            ModelButton openFileButton = new ModelButton();
            openFileButton.Text = "Open File";
            openFileButton.Margin = 5;
            openFileButton.SetSizeRequest(20,20);

            
            
            openFileButton.Show();
            ModelButton saveFileButton = new ModelButton();
            saveFileButton.Text = "Save File";
            saveFileButton.Margin = 5;
            saveFileButton.SetSizeRequest(20,20);


            saveFileButton.Show();
            ModelButton saveAsFileButton = new ModelButton();
            saveAsFileButton.Text = "Save as";
            saveAsFileButton.Margin = 5;
            saveAsFileButton.SetSizeRequest(20,20);
            saveAsFileButton.Show();
            
            newWindowButton.Clicked+=(Events, Args) =>
            {
                new Program(WindowType.Toplevel,null);
            };

            openFileButton.Clicked += openFile;
            saveFileButton.Clicked += saveFile;
            saveAsFileButton.Clicked += saveAsFile;
            
            VBox popoverVbox = new VBox();
            popoverVbox.Margin = 12;
            popoverVbox.ShowAll();

            popoverVbox.Add(newWindowButton);
            popoverVbox.Add(new SeparatorMenuItem
            {
                Visible=true
                
            });
            popoverVbox.Add(openFileButton);
            popoverVbox.Add(new SeparatorMenuItem
            {
                Visible=true
                
            });
            popoverVbox.Add(saveFileButton);
            popoverVbox.Add(saveAsFileButton);
      
            popoverMenu.Add(popoverVbox);
            

            
            MenuButton menuButton = new MenuButton();
            menuButton.Popover = popoverMenu;
            Image image= Gtk.Image.NewFromIconName("view-more-symbolic",IconSize.LargeToolbar);
            menuButton.Image = image;
            headerBar.PackEnd(menuButton);

            Statusbar statusbar = new Statusbar();
            statusBarLabel = new("");
            statusbar.PackStart(statusBarLabel,false,false,5);
            


            PopoverMenu languagePopMenu = new PopoverMenu();
            ListStore languageListStore=new (typeof(string));
            Entry languageSelectorEntry = new();
            EntryCompletion languageSelectorEntryCompletion = new();
            
            
            
            ListBox languageSelectorList = new();
            ScrolledWindow languageScrolledWindow = new();
            VBox languagePopMenuVbox = new();

            foreach (var (key,value) in languageMap)
            {
                var li = new MenuItem{Visible=true,Label = value};
                li.ButtonPressEvent += (sender, eventArgs) =>
                {
                    languageListStore.AppendValues(key);
                    var lm = LanguageManager.Default;
                    var lang = lm.GetLanguage(key);
                    sourceView.Buffer.Language = lang;
                    if (lang != null)
                    {
                        languageSelectorButton.Label = lang.Name;
                        Console.WriteLine(lang.Name);
                    }
                    else
                    {
                        languageSelectorButton.Label = "Text";
                    }
                    
                    
                }; 
                languageSelectorList.Add(li);

            }

            languageSelectorEntryCompletion.Model = languageListStore;
            languageSelectorEntryCompletion.TextColumn = 0;
            languageSelectorEntry.Completion =languageSelectorEntryCompletion ;
            
            
            

            
            // foreach (var key in LanguageManager.Default.SearchPath)
            {
                // Console.WriteLine($"{key} {LanguageManager.Default.Data[key]}");
                
            }
            // var i = new MenuItem {Label = "test1", Visible = true};
            // i.ButtonPressEvent += (sender, eventArgs) => Console.Write("here"); 
            // languageSelectorList.Add(i);
            // languageSelectorList.Add(new MenuItem{Label = "test1",Visible = true});
            //
            // languageSelectorList.Add(new MenuItem{Label = "test1",Visible = true});
            // languageSelectorList.Add(new MenuItem{Label = "test1",Visible = true});
            // languageSelectorList.Add(new MenuItem{Label = "test1",Visible = true});
            // languageSelectorList.Add(new MenuItem{Label = "test1",Visible = true});
            // languageSelectorList.Add(new MenuItem{Label = "test1",Visible = true});
            // languageSelectorList.Add(new MenuItem{Label = "test1",Visible = true});
            // languageSelectorList.Add(new MenuItem{Label = "test1",Visible = true});
            // languageSelectorList.Add(new MenuItem{Label = "test1",Visible = true});
            // languageSelectorList.Add(new MenuItem{Label = "test1",Visible = true});





            languageScrolledWindow.Add(languageSelectorList);
            languageScrolledWindow.SetSizeRequest(100,200);
            languagePopMenuVbox.PackStart(languageSelectorEntry,true,true,5);
            languagePopMenuVbox.PackStart(languageScrolledWindow,true,true,5);
            languagePopMenu.Add(languagePopMenuVbox);
            languagePopMenuVbox.ShowAll();
            languageSelectorEntry.Hide();

            
            
            
            languageSelectorButton = new MenuButton();
            languageSelectorButton.Label ="Text";
            languageSelectorButton.Popover = languagePopMenu;

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


        private List<String> splitText(String s)
        {
            if (s.Length > 0)
            {
                List<String> result = new();
                String temp = "";
     
                String space = "";
                // foreach (var l in s)
                // {
                //     if (String.IsNullOrWhiteSpace(l.ToString())  && l.ToString() != Environment.NewLine)
                //     {
                //         temp += l.ToString();
                //         result.Add(space);
                //         space = "";
                //
                //     }
                //     else 
                //     {
                //         space += l.ToString();
                //         result.Add(temp);
                //         temp = "";
                //     }
                // }
                
                
                
                result.Add(temp);
                // result.Add(space);


                // result.Add(list[list.Count - 1]);

                String spaceConcat = "";
                List<String> finalResult = new();


                // finalResult.RemoveAll(s =>s=="")
                //
                // if (finalResult[finalResult.Count - 1] == " ")

                // finalResult.Add(result[result.Count-1]);

                foreach (var VARIABLE in result)
                {
                    // Console.Write(VARIABLE);

                }

                // Console.WriteLine();



                return result;
            }
            else
            {
                return null;

            }
        }
        



        private void saveFile(object sender,EventArgs args)
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
        
        private void saveAsFile(object sender,EventArgs args)
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
        private void openFile(object sender, EventArgs args)
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
                defaultText = text;
                isChangeSaved = true;
                
            }
            // else if (respone == (int) Gtk.ResponseType.Cancel)
            // {
            //     
            // }
            fileopenDialog.Dispose();


        }

        private void openFileWithArgs(String arg)
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
                defaultText = text;
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
            str.Split("\\n").ToList().ForEach(s =>
                {
                    // if (String.)
                    {
                        // Console.Write("line");
                        
                    }
                    
                }
                );
            
            
            // String str = "\n\n\n";
            // str.Split("(?!^").ToList().ForEach(s => { Console.Write(s); });
            
            Gtk.Application.Init();
            new Program(WindowType.Toplevel,args);
            Gtk.Application.Run();
            // Console.WriteLine("Hello World!");

        }
    }
}
