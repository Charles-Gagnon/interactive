﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine;
using System.Threading.Tasks;
using Microsoft.DotNet.Interactive.Server;

namespace Microsoft.DotNet.Interactive.App.CommandLine
{
    internal static class VSCodeCommand
    {
        public static async Task<int> Do(StartupOptions startupOptions, KernelServer kernelServer, IConsole console)
        {
            var disposable = Program.StartToolLogging(startupOptions);
            var run = kernelServer.RunAsync();
            kernelServer.NotifyIsReady();
            await run;
            return 0;
        }
    }
}
