using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Core
{
    public enum DataState
    {
        Stopped,
        Started
    };

    public class SaveData
    {
        private BinaryWriter binaryWriter;
        private string filePath;
        public  DataState State { get; set; }

        public SaveData() => State = DataState.Stopped;
        
        public void StartSave()
        {
            if (string.IsNullOrEmpty(filePath))
            {
                var mDialog = new FolderBrowserDialog();
                DialogResult result = mDialog.ShowDialog();

                if (result == DialogResult.Cancel) return;

                filePath = mDialog.SelectedPath.Trim();

            }

            var fileName = DateTime.Now.ToString("MM-dd") + "保存数据";
            State = DataState.Started;
            var path = filePath + "\\" + fileName;
            binaryWriter = new BinaryWriter(new FileStream(@path, FileMode.Create, FileAccess.Write));
        }

        public void StopSave()
        {
            binaryWriter.Dispose();
            State = DataState.Stopped;
        }

        public byte[] WriteData(byte[] bytes)
        {
            if (State == DataState.Started) binaryWriter.Write(bytes);
            return bytes;
        }
    }
}
