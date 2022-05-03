// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.Utility.Logging
{
    public abstract class Logger
    {
        public enum Level
        {
            Info = 0,
            Warn = 1,
            Erro = 2
        }
        public Level ShowLevel
        {
            get; set;
        }

        public async Task Log(Level level, string log)
        {
            await LogInternal(level, log, DateTime.Now.ToString("yyyy/MM/ddTHH:mm:ss.fff"));
        }

        public async Task LogInfo(string log)
        {
            await Log(Level.Info, log);
        }
        public async Task LogWarning(string log)
        {
            await Log(Level.Warn, log);
        }
        public async Task LogError(string log)
        {
            await Log(Level.Erro, log);
        }

        protected abstract Task LogInternal(Level level, string log, string timestamp);
    }
}
