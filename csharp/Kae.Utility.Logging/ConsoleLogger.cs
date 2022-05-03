// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.Utility.Logging
{
    public class ConsoleLogger : Logger
    {
        private static ConsoleLogger logger;
        public static Logger CreateLogger()
        {
            if (logger == null)
            {
                logger = new ConsoleLogger() { ShowLevel = Level.Info };
            }
            return logger;
        }

        protected ConsoleLogger()
        {

        }

        protected override async Task LogInternal(Level level, string log, string timestamp)
        {
            if (ShowLevel >= level)
            {
                Console.WriteLine($"[{level}]{timestamp}: {log}");
            }
        }
    }
}

