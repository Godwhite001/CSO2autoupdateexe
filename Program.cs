using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using testdownload;

namespace CSO2自动升级附属程序
{
    class Program
    {

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int x,
        int y,
        int cx,
        int cy,
        int uFlags);

        private const int HWND_TOPMOST = -1;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("winInet.dll")]
        private static extern bool InternetGetConnectedState(
        ref int dwFlag,
        int dwReserved);
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;
            SetWindowPos(hWnd, new IntPtr(HWND_TOPMOST), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);
            Console.Title = "CSO2自动升级附属程序正在运行...";
            Console.WindowWidth = 65; //设置窗体宽度
            Console.BufferWidth = 65; //设置缓存宽度
            Console.WindowHeight = 15;//设置窗体高度
            Console.BufferHeight = 15;//设置缓存高度
            Console.WindowWidth = 65; //重新设置窗体宽度
            System.Int32 dwflag = new int();
            if (!InternetGetConnectedState(ref dwflag,0))
            {
                Console.WriteLine("当前上网设备出现异常！请检查网络环境后再试！");
                Thread.Sleep(10000);
                Environment.Exit(0);
            }

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //创建安全通道
            HttpDldFile df = new HttpDldFile();
            if (File.Exists(System.IO.Path.GetTempPath() + "CSOL2魔改地图系列自动更新程序.exe"))
            {
                File.Delete(System.IO.Path.GetTempPath() + "CSOL2魔改地图系列自动更新程序.exe");
            }
            Console.WriteLine("正在下载主程序文件...");
            if (File.Exists(System.IO.Path.GetTempPath() + "updateexe.zip"))
            {
                File.Delete(System.IO.Path.GetTempPath() + "updateexe.zip");
            }
            df.Download("https://godwhite001.github.io/updateexe.zip", System.IO.Path.GetTempPath() + "updateexe.zip");
            Thread.Sleep(1000);
            if (File.Exists(System.IO.Path.GetTempPath() + "updateexe.zip"))
            {
                
                Console.WriteLine("下载完成!");

            }
            if (File.Exists(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "CSOL2魔改地图系列自动更新程序.exe"))
            {
                File.Delete(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "CSOL2魔改地图系列自动更新程序.exe");
            }
            ZipFile.ExtractToDirectory(System.IO.Path.GetTempPath() + "updateexe.zip", System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
            Console.WriteLine("更新完成!");
            long size = GetFileSize("CSOL2魔改地图系列自动更新程序.exe");
            string realsize = CountSize(size);
            Console.WriteLine("最新客户端文件大小：" + realsize );
            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "CSOL2魔改地图系列自动更新程序.exe");
            Console.WriteLine("最新客户端程序开发者：" + myFileVersionInfo.CompanyName);
            Console.WriteLine("最新客户端文件版本信息："+myFileVersionInfo.FileVersion);
            Console.WriteLine("当前客户端MD5："+GetMD5HashFromFile("CSOL2魔改地图系列自动更新程序.exe"));
            if (File.Exists(System.IO.Path.GetTempPath() + "updateexemd5.txt"))
            {
                File.Delete(System.IO.Path.GetTempPath() + "updateexemd5.txt");
            }
            HttpDldFile df1 = new HttpDldFile();
            df1.Download("https://godwhite001.github.io/updateexemd5.txt", System.IO.Path.GetTempPath() + "updateexemd5.txt");
            string str;
            string txt;
            StreamReader sr = new StreamReader(System.IO.Path.GetTempPath() + "updateexemd5.txt", false);
            str = sr.ReadLine().ToString();
            txt = str;
            sr.Close();
            if (txt != GetMD5HashFromFile("CSOL2魔改地图系列自动更新程序.exe"))
            {
                Console.WriteLine("当前下载的客户端与服务器上的MD5不匹配！警告！服务端已被入侵,即将通知网警介入！正在退出更新程序...");
                // 开发者正在进行自救,如成功则所有服务恢复正常！
                Thread.Sleep(10000);
                Process.Start("http://www.cyberpolice.cn/wfjb/");
                if (File.Exists(System.IO.Path.GetTempPath() + "updateexemd5.txt"))
                {
                    File.Delete(System.IO.Path.GetTempPath() + "updateexemd5.txt");
                }
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("所有信息效验完毕..." + '\n' + "请稍等...即将开启最新客户端！");
                Thread.Sleep(10000);
                Process.Start("CSOL2魔改地图系列自动更新程序.exe");
                if (File.Exists(System.IO.Path.GetTempPath() + "updateexemd5.txt"))
                {
                    File.Delete(System.IO.Path.GetTempPath() + "updateexemd5.txt");
                }
                Environment.Exit(0);
                Console.ReadLine();
            }

        }
        public static long GetFileSize(string sFullName)
        {
            long lSize = 0;
            if (File.Exists(sFullName))
                lSize = new FileInfo(sFullName).Length;
            return lSize;
        }
        public static string CountSize(long Size)
        {
            string m_strSize = "";
            long FactSize = 0;
            FactSize = Size;
            if (FactSize < 1024.00)
                m_strSize = FactSize.ToString("F2") + " Byte";
            else if (FactSize >= 1024.00 && FactSize < 1048576)
                m_strSize = (FactSize / 1024.00).ToString("F2") + " K";
            else if (FactSize >= 1048576 && FactSize < 1073741824)
                m_strSize = (FactSize / 1024.00 / 1024.00).ToString("F2") + " M";
            else if (FactSize >= 1073741824)
                m_strSize = (FactSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " G";
            return m_strSize;
        }
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();


                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
    }
}
