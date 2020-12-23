using Gtk;
using GtkSource;

namespace RoboPad
{
    public class ToolButtonHandler : Toolbar
    {
        public ToolButtonHandler(RoboPad parent,SourceView sourceView)
        {
            ToolbarStyle = ToolbarStyle.Icons;

            ToolButton newToolButton = new ToolButton("NewToolButton");
            newToolButton.IconName = "document-new";
            Insert(newToolButton, 0);
            newToolButton.TooltipText = "New Window";

            ToolButton openToolButton = new ToolButton("OpenToolButton");
            openToolButton.IconName = "document-open";
            Insert(openToolButton, 1);
            openToolButton.TooltipText = " Open";

            ToolButton saveToolButton = new ToolButton("SaveToolButton");
            saveToolButton.IconName = "document-save";
            Insert(saveToolButton, 2);
            saveToolButton.TooltipText = "Save";

            ToolButton saveAsToolButton = new ToolButton("SaveAsToolButton");
            saveAsToolButton.IconName = "document-save-as";
            saveAsToolButton.TooltipMarkup = "Save As";
            Insert(saveAsToolButton, 3);

            ToolButton undoToolButton = new ToolButton("UndoToolButton");
            undoToolButton.IconName = "edit-undo";
            undoToolButton.TooltipMarkup = "Undo Text";
            Insert(undoToolButton, 4);

            ToolButton redoToolButton = new ToolButton("RedoToolButton");
            redoToolButton.IconName = "edit-redo";
            redoToolButton.TooltipMarkup = "Redo Text";
            Insert(redoToolButton, 5);
            
            openToolButton.Clicked += parent.openFile;
            saveToolButton.Clicked += parent.saveFile;
            saveAsToolButton.Clicked += parent.saveAsFile;
            newToolButton.Clicked += (Events, Args) =>
            {
                new RoboPad(WindowType.Toplevel,null);
            };
            undoToolButton.Clicked += (sender, eventArgs) => sourceView.Buffer.Undo();
            redoToolButton.Clicked += (sender, eventArgs) => sourceView.Buffer.Redo();



        }
    }
}