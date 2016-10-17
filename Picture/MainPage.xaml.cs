using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Picture
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            // fileManager();
            //  fileMonth();
            // fileCreat();
            //  readFile();
            // knowLibrary();
            file_picker();
        }
        //文件查询遍历
        async void fileManager()
        {
            StorageFolder picturesFolder = KnownFolders.DocumentsLibrary;

            StringBuilder outputText = new StringBuilder();
            IReadOnlyList<StorageFile> fileList = await picturesFolder.GetFilesAsync();

            outputText.AppendLine("Files");
            foreach (StorageFile file in fileList)
            {
                outputText.Append(file.Name + "\n");
            }
            Debug.WriteLine(outputText);
            IReadOnlyList<StorageFolder> FolderList = await picturesFolder.GetFoldersAsync();
            outputText.AppendLine("Folder:");
            foreach (StorageFolder folder in FolderList)
            {
                outputText.Append(folder.DisplayName + "\n");
            }
            Debug.WriteLine(outputText);
            IReadOnlyList<IStorageItem> itemList = await picturesFolder.GetItemsAsync();
            foreach (IStorageItem item in itemList)
            {
                if (item is StorageFolder)
                {
                    outputText.Append(item.Name + "folder\n");
                }
                else
                {

                    outputText.Append(item.Name + "file\n");
                }


            }
            Debug.WriteLine(outputText);
        }
        //文件按月份输出
        async void fileMonth()
        {
            StorageFolder picturesFolder = KnownFolders.PicturesLibrary;

            StorageFolderQueryResult queryResult =
                picturesFolder.CreateFolderQuery(CommonFolderQuery.GroupByMonth);

            IReadOnlyList<StorageFolder> folderList =
                await queryResult.GetFoldersAsync();

            StringBuilder outputText = new StringBuilder();

            foreach (StorageFolder folder in folderList)
            {
                IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();

                // Print the month and number of files in this group.
                outputText.AppendLine(folder.Name + " (" + fileList.Count + ")");

                foreach (StorageFile file in fileList)
                {
                    // Print the name of the file.
                    outputText.AppendLine("   " + file.Name);
                }
                Debug.WriteLine(outputText);
            }
        }
        //文件的创建
        async void fileCreat()
        {
            Windows.Storage.StorageFolder storeFolder = Windows.Storage.ApplicationData.Current.LocalFolder;            
            Windows.Storage.StorageFile sampleFile = await storeFolder.CreateFileAsync("sample.text", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(sampleFile,"Zhu_Ming_Hao");
            var buffer = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(
        "What fools these mortals be", Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
            await Windows.Storage.FileIO.WriteBufferAsync(sampleFile, buffer);

        }
        //文件读取
        async void readFile()
        {
            StringBuilder str = new StringBuilder();
            StorageFolder foler = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await foler.GetFileAsync("sample.text");
             //文件转化成流的操作
            var stream = await sampleFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
            str.Append(stream.ToString());
            //文件转化字符串
            string ss =  await FileIO.ReadTextAsync(sampleFile);
            //文件转创建句柄
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(sampleFile);

            Debug.WriteLine(ss);
            //不太常用 获取buffer的长度
            using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
            {
                string text = dataReader.ReadString(buffer.Length);
            }
        }
         async void knowLibrary()
        {
            StorageLibrary pic = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);

            IObservableVector<StorageFolder> myPictureFolders = pic.Folders;
            //将现有文件添加到文件库中
            StorageFolder newFolder = await pic.RequestAddFolderAsync();
            //文件夹中移除子文件夹
            //bool result = await picture.RequestRemoveFolderAsync();   
            pic.DefinitionChanged += MyPictures_DefinitionChanged;
        }

        private void MyPictures_DefinitionChanged(StorageLibrary sender, object args)
        {
            throw new NotImplementedException();
            Debug.WriteLine("---------------");
        }

        async void addDataToLibrary()
        {
            StorageFolder app_folder = ApplicationData.Current.LocalFolder;
               StorageFolder test = await app_folder.CreateFolderAsync("test");
            StorageFile sourceFile = await test.GetFileAsync("TestImage.jpg");
            StorageFile destinationFile = await KnownFolders.CameraRoll.CreateFileAsync("MyTestImage.jpg");

            using (var sourceStream = (await sourceFile.OpenReadAsync()).GetInputStreamAt(0))
            {
                using (var destinationStream = (await destinationFile.OpenAsync(FileAccessMode.ReadWrite)).GetOutputStreamAt(0))
                {
                    await RandomAccessStream.CopyAndCloseAsync(sourceStream, destinationStream);
                }
            }

        }
        /*     图片库
     GetFilesAsync(CommonFileQuery.OrderByDate)
     音乐库
     GetFilesAsync(CommonFileQuery.OrderByName)
     GetFoldersAsync(CommonFolderQuery.GroupByArtist)
     GetFoldersAysnc(CommonFolderQuery.GroupByAlbum)
     GetFoldersAysnc(CommonFolderQuery.GroupByAlbumArtist)
     GetFoldersAsync(CommonFolderQuery.GroupByGenre)
     视频库
     GetFilesAsync(CommonFileQuery.OrderByDate*/

        //await KnownFolders.PicturesLibrary.GetFilesAsync() 如果在sd卡中存在pic 文件库， 方法调用发挥结果会包含两个位置的查询结果
       
        async void file_picker()
        {
            //创建一个文件选取器 设置相应的属性ViewMode、SuggestedStartLocation 和 FileTypeFilter。
            var picker = new FileOpenPicker();
            //视图模式
            picker.ViewMode = PickerViewMode.List;
            //建议开始的浏览位置
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            //搜索文件属性限制
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            //文件选取后的回掉结果 file 对象
           // StorageFile file = await picker.PickSingleFileAsync();
            //文件转成流的操作
             //   var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
             //文件多选模式
             var files = await picker.PickMultipleFilesAsync();
            StringBuilder str = new StringBuilder();
            //if (file != null)
            //{
            //    this.block.Text = "pick:" + file.Name;
            //}else
            //{
            //    this.block.Text = "open filed";
            //}
            if (files.Count > 0)
            {
                foreach( StorageFile file in files)
                {
                    str.Append(file.Name);
                }
                block.Text = str.ToString();
            }else
            {
                block.Text = "nothing you selected";
            }

        }

        async void file_save_Picker()
        {

        }
    }

}
