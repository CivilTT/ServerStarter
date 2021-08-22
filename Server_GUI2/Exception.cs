using log4net;
using System;
using System.Reflection;

namespace Server_GUI2
{
    class DowngradeException : Exception
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DowngradeException(string message) : base(message)
        {
            logger.Info(message);
            Console.WriteLine(App.end_str);
        }
    }

    class GitException: Exception
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public GitException(string message) : base(message)
        {
            logger.Info(message);
            Console.WriteLine(App.end_str);
        }
    }

    class UserSelectException : Exception
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UserSelectException(string message) : base(message)
        {
            logger.Error(message);
            Console.WriteLine(App.end_str);
        }
    }

    class ServerException : Exception
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ServerException(string message) : base(message)
        {
            logger.Error(message);
            Console.WriteLine(App.end_str);
        }
    }

    class WinCommandException : Exception
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public WinCommandException(string message) : base(message)
        {
            logger.Error(message);
            Console.WriteLine(App.end_str);
        }
    }

    class ArgumentException : Exception
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ArgumentException(string message) : base(message)
        {
            logger.Error(message);
            Console.Write(App.end_str);
        }
    }

    class IOException : Exception
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IOException(string message) : base(message)
        {
            logger.Error(message);
            Console.WriteLine(App.end_str);
        }
    }

    class DownloadException : Exception
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DownloadException(string message) : base(message)
        {
            logger.Error(message);
            Console.WriteLine(App.end_str);
        }
    }
}
