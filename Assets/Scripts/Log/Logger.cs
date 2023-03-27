using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maple
{
    public enum LOGGER_LEVEL
    {
        LOGGER_LEVEL_ERROR = 0,
        LOGGER_LEVEL_WARNING = 1,
        LOGGER_LEVEL_INFO = 2,
        LOGGER_LEVEL_DEBUG = 3,
    }

    public class Logger
    {
        private static Logger instance_;

        public static void Report(LOGGER_LEVEL level, string info)
        {
            if (instance_ == null)
            {
                instance_ = new Logger();
            }

            switch(level)
            {
                case LOGGER_LEVEL.LOGGER_LEVEL_ERROR:
                    {
                        instance_.Error(info);
                        break;
                    }
                case LOGGER_LEVEL.LOGGER_LEVEL_WARNING:
                case LOGGER_LEVEL.LOGGER_LEVEL_INFO:
                case LOGGER_LEVEL.LOGGER_LEVEL_DEBUG:
                    {
                        instance_.Info(info);
                        break;
                    }

            }
        }

        private void Info(string info)
        {
            Debug.Log(info);
        }

        private void Error(string info)
        {
            Debug.LogError(info);
        }
    }
}
