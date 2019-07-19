using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FilterTest
{
    /// <summary>
    /// 优化的算法
    /// </summary>
    public class BadWordsFilter2
    {
        private HashSet<string> hash = new HashSet<string>();

        private UInt16[] fastCheck = new UInt16[char.MaxValue];
        private UInt16[] startLength = new UInt16[char.MaxValue];
        private UInt16[] endLength = new UInt16[char.MaxValue];

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
                fastCheck[word[i]] |= (byte)(1 << i);
            }

            UInt16 mask = (UInt16)(1 << word.Length - 1);
            //以x开始的字符的长度
            startLength[word[0]] |= mask;
            //以x结束的字符的长度
            endLength[word[word.Length - 1]] |= mask;

            hash.Add(word);
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
                    if ((fastCheck[current] & mask) == 0)
                    {
                        index += count;
                        break;
                    }
                    ++count;
                    if ((startLength[begin] & mask) > 0 && (endLength[current] & mask) > 0)
                    {
                        string sub = text.Substring(index, count);
                        if (hash.Contains(sub))
                        {
                            //index += (count - 1);
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
                    if ((fastCheck[current] & mask) == 0)
                    {
                        index += count;
                        break;
                    }
                    ++count;
                    if ((startLength[begin] & mask) > 0 && (endLength[current] & mask) > 0)
                    {
                        string sub = text.Substring(index, count);
                        if (hash.Contains(sub))
                        {
                            index += (count - 1);
                            return sub;
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
                    if ((fastCheck[current] & mask) == 0)
                    {
                        index += count;
                        break;
                    }
                    ++count;
                    if ((startLength[begin] & mask) > 0 && (endLength[current] & mask) > 0)
                    {
                        string sub = text.Substring(index, count);
                        if (hash.Contains(sub))
                        {
                            index += (count - 1);
                            yield return sub;
                            break;
                        }
                    }
                }
            }
        }

    }

}
