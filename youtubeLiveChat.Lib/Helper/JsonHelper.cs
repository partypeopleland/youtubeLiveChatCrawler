using System.Diagnostics;

namespace youtubeLiveChat.Lib.Helper;

public static class JsonHelper
{
    #region PUBLIC METHODS

    /// <summary>
    /// Try to get the json data value and not throw exception when there is not the key. Return default value  if there is not the key.
    /// </summary>
    /// <param name="jsonData">Raw json data.</param>
    /// <param name="key">Try string of json data key.</param>
    /// <param name="defaultValue">If there is not the key, return this value. </param>
    /// <returns>Return default value if there is not the key.</returns>
    public static object TryGetValue(dynamic jsonData, string key, object defaultValue = null)
    {
        object ret;
        try
        {
            ret = jsonData[key];
            if (ret == null) ret = defaultValue;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(string.Format("Try get value error:{0}", ex.Message));
            return defaultValue;
        }

        return ret;
    }

    /// <summary>
    /// Try to get the json data value and not throw exception when there is not the key. Return default value  if there is not the key.
    /// </summary>
    /// <param name="jsonData">Raw json data.</param>
    /// <param name="idx">Try index of json data array.</param>
    /// <param name="defaultValue">If there is not the key, return this value. </param>
    /// <returns>Return default value if there is not the key.</returns>
    public static object TryGetValue(dynamic jsonData, int idx, object defaultValue = null)
    {
        object ret;
        try
        {
            ret = jsonData[idx];
            if (ret == null) ret = defaultValue;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(string.Format("Try get value error:{0}", ex.Message));
            return defaultValue;
        }

        return ret;
    }

    public static object TryGetValueByXPath(dynamic jsonData, string xPath, object defaultValue = null)
    {
        object ret = jsonData;
        var keys = xPath.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var k in keys)
        {
            var idx = -1;
            if (int.TryParse(k, out idx))
                ret = TryGetValue(ret, idx);
            else
                ret = TryGetValue(ret, k);

            if (ret == null)
            {
                ret = defaultValue;
                break;
            }
        }

        return ret;
    }

    #endregion
}