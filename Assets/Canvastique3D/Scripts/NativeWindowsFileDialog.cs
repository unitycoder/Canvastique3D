#if UNITY_EDITOR || UNITY_STANDALONE_WIN

using System.Windows.Forms;

public class NativeWindowsFileDialog
{
    public string OpenFileLoader(string filter)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Select a glTF file",
            Filter = filter
        };

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            return openFileDialog.FileName;
        }

        return string.Empty;
    }
}

#endif