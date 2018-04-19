using Microsoft.WindowsAPICodePack.Dialogs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace WpfHandGazeDistance.Helpers
{
    public class FileManager
    {
        public static string OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) return openFileDialog.FileName;
            return null;
        }

        public static string OpenFileDialog(string fileType)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true && openFileDialog.FileName.EndsWith(fileType))
                return openFileDialog.FileName;
            return null;
        }

        public static string SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }
            return null;
        }

        public static string SelectFolderDialog()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog {IsFolderPicker = true};

            return dialog.ShowDialog() == CommonFileDialogResult.Ok ? dialog.FileName : null;
        }
    }
}
