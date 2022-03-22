using log4net;
using System;
using System.Reflection;
using Server_GUI2.Windows.MessageBox;
using Server_GUI2.Windows.MessageBox.Back;
using MW = ModernWpf;

namespace Server_GUI2
{
    class ServerStarterException<T> : Exception where T : Exception 
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public T Exception;

        public ServerStarterException(T exception) : base(exception.Message)
        {
            logger.Error(exception.Message);
            Console.WriteLine(App.end_str);
            Exception = exception;
        }

        public static void ShowError<Ex>(string showMessage, Ex ex) where Ex : Exception
        {
            CustomMessageBox.Show(showMessage, ButtonType.OK, Image.Error);

            throw new ServerStarterException<Ex>(ex);
        }
    }

    class DowngradeException : Exception
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DowngradeException(string message) : base(message)
        {
            logger.Info(message);
            Console.WriteLine(App.end_str);
            //ServerStarterException<IOException>.ShowError("", new IOException(""));
        }
    }

    class GitException: Exception
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public GitException(string message) : base(message)
        {
            logger.Info(message);
            Console.WriteLine(App.end_str);
        }
    }

    class UserSelectException : Exception
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UserSelectException(string message) : base(message)
        {
            logger.Error(message);
            Console.WriteLine(App.end_str);
        }
    }

    class ServerException : Exception
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ServerException(string message) : base(message)
        {
            logger.Error(message);
            Console.WriteLine(App.end_str);
        }
    }

    class WinCommandException : Exception
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public WinCommandException(string message) : base(message)
        {
            logger.Error(message);
            Console.WriteLine(App.end_str);
        }
    }

    class ArgumentException : Exception
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ArgumentException(string message) : base(message)
        {
            logger.Error(message);
            Console.Write(App.end_str);
        }
    }

    class IOException : Exception
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IOException(string message) : base(message)
        {
            logger.Error(message);
            Console.WriteLine(App.end_str);
        }
    }

    class DownloadException : Exception
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DownloadException(string message) : base(message)
        {
            logger.Error(message);
            Console.WriteLine(App.end_str);
        }
    }
}
