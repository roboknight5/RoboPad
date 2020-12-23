using Gtk;
using System;
namespace RoboPad
{
    public class PopOverMenuHandler : PopoverMenu
    {
        public PopOverMenuHandler(RoboPad parent)
        {
            // PopoverMenu popoverMenu = new PopoverMenu();
            
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
                new RoboPad(WindowType.Toplevel,null);
            };

            openFileButton.Clicked += parent.openFile;
            saveFileButton.Clicked += parent.saveFile;
            saveAsFileButton.Clicked += parent.saveAsFile;
            
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

            Add(popoverVbox);
        }
    }
}