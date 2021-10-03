using log4net;
using System;
using System.Reflection;

namespace Server_GUI2
{
    class DowngradeException : Exception
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DowngradeException(string message) : base(message)
        {
            logger.Info(message);
            Console.WriteLine(App.end_str);
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
