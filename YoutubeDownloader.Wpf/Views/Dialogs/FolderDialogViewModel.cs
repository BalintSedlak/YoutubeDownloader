using Ookii.Dialogs.Wpf;

namespace YoutubeDownloader.Wpf.Views.Dialogs
{
    public class FolderDialogViewModel
    {
        public string ShowDialog()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.ShowNewFolderButton = true;

            var dialogResult = dialog.ShowDialog();

            if (dialogResult != true)
            {
                //Todo
                return string.Empty;
            }

            return dialog.SelectedPath;
        }
    }
}
