using System;
using Gtk;
using GtkSource;

namespace RoboPad
{
    public class LanguageSelectorButton : MenuButton
    {
        public LanguageSelectorButton(SourceView sourceView)
        {
             PopoverMenu languagePopMenu = new PopoverMenu();
            ListStore languageListStore=new (typeof(string));
            Entry languageSelectorEntry = new();
            EntryCompletion languageSelectorEntryCompletion = new();
            
            
            
            ListBox languageSelectorList = new();
            ScrolledWindow languageScrolledWindow = new();
            VBox languagePopMenuVbox = new();

            foreach (var (key,value) in LanguageMap.languageMap)
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
                        Label = lang.Name;
                        Console.WriteLine(lang.Name);
                    }
                    else
                    {
                        Label = "Text";
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

            
            
            
            Label ="Text";
            Popover = languagePopMenu;
        }
    }
}