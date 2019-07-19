using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sinan.Util;
using HashSet = Sinan.Util.HashStringSet;

namespace FilterTest
{
    /// <summary>
    /// 再优化(替换HashSet)
    /// </summary>
    public class BadWordsFilter4
    {
        private HashSet m_hashSet = new HashSet();

        private UInt16[] m_fastCheck = new UInt16[char.MaxValue];
        private UInt16[] m_startLength = new UInt16[char.MaxValue];
        private UInt16[] m_endLength = new UInt16[char.MaxValue];

        private int maxWordLength = 0;
        private int minWordLength = int.MaxValue;

        public void AddKey(string word)
        {
            if (word.Length > 16)
            {
                throw new Exception("参数最大16个字符");
            }

            maxWordLength = Math.Max(maxWordLength, word.Length);
            minWordLength = Math.Min(minWordLength, word.Length);
            //字符出现的位置(1-16),
            for (int i = 0; i < word.Length; i++)
            {
                m_fastCheck[word[i]] |= (UInt16)(1 << i);
            }

            UInt16 mask = (UInt16)(1 << word.Length - 1);
            //以x开始的字符的长度
            m_startLength[word[0]] |= mask;
            //以x结束的字符的长度
            m_endLength[word[word.Length - 1]] |= mask;

            m_hashSet.Add(word);
        }

        public bool HasBadWord(string text)
        {
            for (int index = 0; index < text.Length; index++)
            {
                int count = 0;
                int maxIndex = Math.Min(maxWordLength + index, text.Length);
                char begin = text[index];

                for (int j = index; j < maxIndex; j++)
                {
                    char current = text[j];
                    UInt16 mask = (UInt16)(1 << count);
                    //先判断字符出现的位置是否正确
                    if ((m_fastCheck[current] & mask) == 0)
                    {
                        index += count;
                        break;
                    }
                    ++count;
                    //再判断首字符和尾字符的长度是否正确.
                    if ((m_startLength[begin] & mask) > 0 && (m_endLength[current] & mask) > 0)
                    {
                        //进行hash比较
                        if (m_hashSet.Contains(text, index, count))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public string FindOne(string text)
        {
            for (int index = 0; index < text.Length; index++)
            {
                int count = 0;
                int maxIndex = Math.Min(maxWordLength + index, text.Length);
                char begin = text[index];

                for (int j = index; j < maxIndex; j++)
                {
                    char current = text[j];
                    UInt16 mask = (UInt16)(1 << count);
                    //先判断字符出现的位置是否正确
                    if ((m_fastCheck[current] & mask) == 0)
                    {
                        index += count;
                        break;
                    }
                    ++count;
                    //再判断首字符和尾字符的长度是否正确.
                    if ((m_startLength[begin] & mask) > 0 && (m_endLength[current] & mask) > 0)
                    {
                        //进行hash比较
                        if (m_hashSet.Contains(text, index, count))
                        {
                            return text.Substring(index, count);
                        }
                    }
                }
            }
            return string.Empty;
        }

        public IEnumerable<string> FindAll(string text)
        {
            for (int index = 0; index < text.Length; index++)
            {
                int count = 0;
                int maxIndex = Math.Min(maxWordLength + index, text.Length);
                char begin = text[index];
                for (int j = index; j < maxIndex; j++)
                {
                    char current = text[j];
                    UInt16 mask = (UInt16)(1 << count);
                    if ((m_fastCheck[current] & mask) == 0)
                    {
                        index += count;
                        break;
                    }
                    ++count;
                    if ((m_startLength[begin] & mask) > 0 && (m_endLength[current] & mask) > 0)
                    {
                        if (m_hashSet.Contains(text, index, count))
                        {
                            yield return text.Substring(index, count);
                            index += (count - 1);
                            break;
                        }
                    }
                }
            }
        }

        public string Replace(string text, char maskChar = '*')
        {
            char[] chars = null;
            for (int index = 0; index < text.Length; index++)
            {
                int count = 0;
                int maxIndex = Math.Min(maxWordLength + index, text.Length);
                char begin = text[index];

                for (int j = index; j < maxIndex; j++)
                {
                    char current = text[j];
                    UInt16 mask = (UInt16)(1 << count);
                    if ((m_fastCheck[current] & mask) == 0)
                    {
                        index += count;
                        break;
                    }
                    ++count;
                    if ((m_startLength[begin] & mask) > 0 && (m_endLength[current] & mask) > 0)
                    {
                        if (m_hashSet.Contains(text, index, count))
                        {
                            if (chars == null) chars = text.ToArray();
                            for (int i = index; i < index + count; i++)
                            {
                                chars[i] = maskChar;
                            }
                            index += (count - 1);
                            break;
                        }
                    }
                }
            }
            return chars == null ? text : new string(chars);
        }
    }

}
