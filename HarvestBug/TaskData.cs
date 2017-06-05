using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestBug
{
    public enum TaskType
    {
        message
    }

    public struct TaskData
    {
        public string text;
        public string attachments;
        public string max;
        public string current;
        public TaskType type;
    }
}
