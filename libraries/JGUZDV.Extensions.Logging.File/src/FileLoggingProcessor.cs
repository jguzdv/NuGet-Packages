using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace JGUZDV.Extensions.Logging.File
{
    /// <summary>
    /// 
    /// </summary>
    internal class FileLoggingProcessor : IDisposable
    {
        private readonly Channel<string> _channel;

        public FileLoggingProcessor(Channel<string> channel)
        {
            _channel = channel;
        }

        public void Dispose()
        {
            
        }
    }
}
