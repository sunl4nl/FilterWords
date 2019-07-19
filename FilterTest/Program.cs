using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FilterTest
{
    class Program
    {
        static string text = "名字加fuck日本人妖";
        static HashFilter hf = new HashFilter();
        static TrieFilter tf = new TrieFilter();
        static Sinan.Util.TrieFilter tf2 = new Sinan.Util.TrieFilter();

        static BadWordsFilter bf = new BadWordsFilter();
        static BadWordsFilter2 bf2 = new BadWordsFilter2();
        static BadWordsFilter3 bf3 = new BadWordsFilter3();
        static BadWordsFilter4 bf4 = new BadWordsFilter4();

        //从文件读取关键字.
        static void ReadBadWord()
        {
            using (StreamReader sw = new StreamReader(File.OpenRead("BadWord.txt")))
            {
                Random random = new Random();
                string key = sw.ReadLine();
                while (key != null)
                {
                    if (key != string.Empty)
                    {
                        hf.AddKey(key);
                        tf.AddKey(key);
                        tf2.AddKey(key);

                        bf.AddKey(key);
                        bf2.AddKey(key);
                        bf3.AddKey(key);
                        bf4.AddKey(key);
                    }
                    key = sw.ReadLine();
                }
            }
        }

        unsafe static int GetStringHashCode(String item, int offset, int len)
        {
            if (item == null)
            {
                return 0;
            }
            fixed (char* str = item)
            {
                char* chPtr = str + offset;
                int num = 0x15051505;
                int num2 = num;
                int* numPtr = (int*)chPtr;
                if ((len & 0x01) == 0 || len + offset == item.Length)
                {
                    for (int i = len; i > 0; i -= 4)
                    {
                        num = (((num << 5) + num) + (num >> 0x1b)) ^ numPtr[0];
                        if (i <= 2)
                        {
                            break;
                        }
                        num2 = (((num2 << 5) + num2) + (num2 >> 0x1b)) ^ numPtr[1];
                        numPtr += 2;
                    }
                    return (num + (num2 * 0x5d588b65)) & 0x7fffffff;
                }
                else
                {
                    int t1, t2;
                    for (int i = len; i > 0; i -= 4)
                    {
                        if (i <= 2)
                        {
                            t1 = numPtr[0] & 0xffff;
                        }
                        else
                        {
                            t1 = numPtr[0];
                        }
                        num = (((num << 5) + num) + (num >> 0x1b)) ^ t1;
                        if (i <= 2)
                        {
                            break;
                        }

                        if (i <= 3)
                        {
                            t1 = numPtr[1] & 0xffff;
                        }
                        else
                        {
                            t2 = numPtr[1];
                        }
                        num2 = (((num2 << 5) + num2) + (num2 >> 0x1b)) ^ t1;
                        numPtr += 2;
                    }
                    return (num + (num2 * 0x5d588b65)) & 0x7fffffff;
                }
            }
        }
        static void TestHash()
        {
            for (int i = 0; i < 10000; i++)
            {
                hf.FindOne(text);
            }
        }

        static void TestTrie()
        {
            for (int i = 0; i < 10000; i++)
            {
                tf.FindOne(text);
            }
        }


        static void TestHasBadWord0()
        {
            for (int i = 0; i < 10000; i++)
            {
                bf.HasBadWord(text);
            }
        }

        static void TestHasBadWord2()
        {
            for (int i = 0; i < 10000; i++)
            {
                bf2.HasBadWord(text);
            }
        }

        static void TestHasBadWord3()
        {
            for (int i = 0; i < 10000; i++)
            {
                bf3.HasBadWord(text);
            }
        }

        static void TestHasBadWord4()
        {
            for (int i = 0; i < 10000; i++)
            {
                bf4.HasBadWord(text);
            }
        }

        static void TestHasBadWord1()
        {
            for (int i = 0; i < 10000; i++)
            {
                tf.HasBadWord(text);
            }
        }

        static void Main(string[] args)
        {

            ReadBadWord();
            CodeTimer.Initialize();
            using (StreamReader sw = new StreamReader(File.OpenRead("Talk.txt")))
            {
                string key = sw.ReadLine();
                while (key != null)
                {
                    if (key != string.Empty)
                    {
                        Console.WriteLine("-------------开始测试-------------");
                        Console.WriteLine(key);
                        text = key;
                        //CodeTimer.Time("原始", 50, TestHasBadWord0);
                        //CodeTimer.Time("优化的算法", 50, TestHasBadWord2);
                        //CodeTimer.Time(" 优化hash和BitArray", 50, TestHasBadWord3);
                        //CodeTimer.Time("双优化", 50, TestHasBadWord4);
                        //CodeTimer.Time("Trie", 50, TestHasBadWord1);


                        foreach (var word in bf4.FindAll(text))
                        {
                            Console.WriteLine(word);
                        }
                        Console.WriteLine(bf4.HasBadWord(text));

                        //Console.WriteLine("推荐使用BadWordsFilter4");
                    }
                    key = sw.ReadLine();
                }
            }

            Console.ReadKey();
        }
    }
}
