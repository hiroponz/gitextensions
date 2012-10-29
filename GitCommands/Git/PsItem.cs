using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitCommands
{
    public sealed class PsItem
    {
        public string Command { get; set; }
        public int Pid { get; set; }
        public int WinPid { get; set; }
    }
}
