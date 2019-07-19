using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FilterTest
{
    public class HashFilter
    {
        int m_maxLen; //关键字最大长度
        HashSet<string> m_keys = new HashSet<string>();

        /// <summary>
        /// 插入新的Key.
        /// </summary>
        /// <param name="name"></param>
        public void AddKey(string key)
        {
            if ((!string.IsNullOrEmpty(key)) && m_keys.Add(key) && key.Length > m_maxLen)
            {
                m_maxLen = key.Length;
            }
        }


        /// <summary>
        /// 检查是否包含非法字符
        /// </summary>
        /// <param name="text">输入文本</param>
        /// <returns>找到的第1个非法字符.没有则返回string.Empty</returns>
        public string FindOne(string text)
        {
            for (int len = 1; len <= text.Length; len++)
            {
                int maxIndex = text.Length - len;
                for (int index = 0; index <= maxIndex; index++)
                {
                    string key = text.Substring(index, len);
                    if (m_keys.Contains(key))
                    {
                        return key;
                    }
                }
            }
            return string.Empty;
        }

        //查找所有非法字符
        public IEnumerable<string> FindAll(string text)
        {
            for (int len = 1; len <= text.Length; len++)
            {
                int maxIndex = text.Length - len;
                for (int index = 0; index <= maxIndex; index++)
                {
                    string key = text.Substring(index, len);
                    if (m_keys.Contains(key))
                    {
                        yield return key;
                    }
                }
            }
        }

        /// <summary>
        /// 替换非法字符
        /// </summary>
        /// <param name="text"></param>
        /// <param name="c">用于代替非法字符</param>
        /// <returns>替换后的字符串</returns>
        public string Replace(string text, char c = '*')
        {
            int maxLen = Math.Min(m_maxLen, text.Length);
            for (int len = 1; len <= maxLen; len++)
            {
                int maxIndex = text.Length - len;
                for (int index = 0; index <= maxIndex; index++)
                {
                    string key = text.Substring(index, len);
                    if (m_keys.Contains(key))
                    {
                        int keyLen = key.Length;
                        text = text.Replace(key, new string(c, keyLen));
                        index += (keyLen - 1);
                    }
                }
            }
            return text;
        }
    }
}
