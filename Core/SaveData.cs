using Core.Interface;
using System;
using System.IO;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Messaging;

namespace Core
{
    public enum DataState
    {
        Stopped,
        Started
    };

    public class SaveData : ISaveData<byte[]>
    {
        private BinaryWriter binaryWriter;
        private string filePath;
        public  DataState State { get; set; }
        private int count;
        private int gestureCount;
        private int flag;
        private int[] gestureKind;

        private string[] gestureName = {"向左滑手势", "向右滑手势", "左边双击手势", "右边双击手势"};

        private FileStream fileStream;

        private readonly string localPath;
        private readonly string name;

        private string temp = string.Empty;

        //public SaveData() => State = DataState.Stopped;

        public SaveData(string name, string localPath)
        {
            this.localPath = localPath;
            this.name = name;
            count = 0;
            gestureCount = 0;
            flag = 0;
            gestureKind = new int[60];

            Random random = new Random();

            for (int i = 0; i < 60; i++)
            {
                gestureKind[i] = random.Next(1, 5);
            }
        }
        
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
            fileStream.Dispose();
            binaryWriter.Dispose();
            State = DataState.Stopped;
        }

        public byte[] WriteData(byte[] bytes)
        {
            if (State == DataState.Started) binaryWriter.Write(bytes);
            return bytes;
        }

        public void WriteProcess(byte[] bytes)
        {

            if (count == 0)
            {
                if (gestureCount > 59)
                {
                    if (flag == 0)
                    {
                        Messenger.Default.Send("全部手势采集完成", "PresentUpdate");
                        flag = 1;
                    }
                    return;
                }

                if (gestureCount != 0)
                {
                    fileStream.Dispose();
                    binaryWriter.Dispose();
                }

                temp = gestureName[gestureKind[gestureCount] - 1].ToString();
                var fileName = temp + "_" + name + "_" + gestureCount.ToString();
                var path = localPath + "\\" + fileName;
                fileStream = new FileStream(@path, FileMode.Create, FileAccess.Write);
                binaryWriter = new BinaryWriter(fileStream);

                var temp2 = gestureKind[gestureCount];

                switch (temp2)
                {
                    case 1: Messenger.Default.Send(1, "LeftMove");break;
                    case 2: Messenger.Default.Send(2, "RightMove"); break;
                    case 3: Messenger.Default.Send(3, "DoubleLeftMove"); break;
                    case 4: Messenger.Default.Send(4, "DoubleRightMove");break;
                    default:break;
                }

                byte[] temp3 = {0x00, 0x9F, 0xE1};
                Messenger.Default.Send(temp3, "ChangeBackground");
                binaryWriter.Write(bytes);

                gestureCount++;
            }
            else if (count > 0 && count < 40)
            {
                if (flag == 0)
                {
                    Messenger.Default.Send(temp + "准备", "PresentUpdate");
                    Messenger.Default.Send("3秒", "PresentUpdate2");
                    flag = 1;
                }

                binaryWriter.Write(bytes);
            }
            else if (count == 40)
            {
                flag = 0;
                binaryWriter.Write(bytes);
            }
            else if (count > 40 && count < 80)
            {
                if (flag == 0)
                {
                    Messenger.Default.Send(temp + "准备", "PresentUpdate");
                    Messenger.Default.Send("2秒", "PresentUpdate2");
                    flag = 1;
                }
                binaryWriter.Write(bytes);
            }
            else if (count == 80)
            {
                flag = 0;
                binaryWriter.Write(bytes);
            }
            else if (count > 80 && count < 120)
            {
                if (flag == 0)
                {
                    Messenger.Default.Send(temp + "准备", "PresentUpdate");
                    Messenger.Default.Send("1秒", "PresentUpdate2");
                    flag = 1;
                }
                binaryWriter.Write(bytes);
            }
            else if (count == 120)
            {
                flag = 0;
                Messenger.Default.Send(temp + "采集中..." , "PresentUpdate");
                Messenger.Default.Send("进度 " + gestureCount.ToString() + "/60", "PresentUpdate2");

                byte[] temp3 = { 0xC1, 0xB1, 0x2A };
                Messenger.Default.Send(temp3, "ChangeBackground");

                binaryWriter.Write(bytes);
            }
            else if (count > 120 && count < 40 * 6)
            {
                 binaryWriter.Write(bytes);
            }
            else
            {
                count = 0;
                binaryWriter.Write(bytes);
                return;
            }

            count++;

        }
    }
}
