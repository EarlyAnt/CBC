using EarlyAnt.Framework.Log4U;
using EarlyAnt.Framework.Util;
using System;

public class Logger
{
    private static Logger instance;
    private SimpleLog debugLog = new SimpleLog("debug.txt", 10);
    private SimpleLog errorLog = new SimpleLog("debug.txt", 10);

    public static Logger GetInstance()
    {
        if (instance == null)
            instance = new Logger();

        return instance;
    }

    public void Debug(string content)
    {
        debugLog.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToyyyyMMddHHmmssfff(), content));
    }

    public void Error(string content)
    {
        errorLog.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToyyyyMMddHHmmssfff(), content));
    }
}
