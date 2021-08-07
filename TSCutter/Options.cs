using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TSCutter
{
    class Options
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public string InputDur { get; set; }
        public string StartTime { get; set; } = "0";
        public string Duration { get; set; } = "0";
        public string StopTime { get; set; } = "0";
        public override string ToString()
        {
            return $"{{\"InputFile\":{Input},\"OutputFile\":{Output},\"InputDur\":{InputDur},\"StartTime\":{StartTime},\"Duration\":{Duration},\"StopTime\":{StopTime}}}";
        }
    }
}
