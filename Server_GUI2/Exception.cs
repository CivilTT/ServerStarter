using System;

namespace Server_GUI2
{
    class DowngradeException : Exception
    {
        public DowngradeException() : base()
        {
            //
        }

        public DowngradeException(string message) : base(message)
        {
            //
        }

        public DowngradeException(string message, Exception innerException) : base(message, innerException)
        {
            //
        }
    }

    class GitException: Exception
    {
        public GitException() : base()
        {
            //
        }

        public GitException(string message) : base(message)
        {
            //
        }

        public GitException(string message, Exception innerException) : base(message, innerException)
        {
            //
        }
    }

    class UserSelectException : Exception
    {
        public UserSelectException() : base()
        {
            //
        }

        public UserSelectException(string message) : base(message)
        {
            //
        }

        public UserSelectException(string message, Exception innerException) : base(message, innerException)
        {
            //
        }
    }

    class ServerException : Exception
    {
        public ServerException() : base()
        {
            //
        }

        public ServerException(string message) : base(message)
        {
            //
        }

        public ServerException(string message, Exception innerException) : base(message, innerException)
        {
            //
        }
    }
}
