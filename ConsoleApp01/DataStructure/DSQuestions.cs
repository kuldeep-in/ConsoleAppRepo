using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp01.DataStructure
{
    public class DSQuestions
    {
        public static void ReverseString(string str)
        {

            char[] charArray = str.ToCharArray();
            for (int i = 0, j = str.Length - 1; i < j; i++, j--)
            {
                charArray[i] = str[j];
                charArray[j] = str[i];
            }
            string reversedstring = new string(charArray);
            Console.WriteLine(reversedstring);
        }

        public static void comparestring(string par01, string par02)
        {
            int len = par01.Length;
            string op01 = "1";
            int count = 1;
            bool matchfound = false;
            int matchlocation = 0;

            for (int i = 1; i < len; i++)
            {
                matchfound = false;
                for (int j = 0; j < i; j++)
                {
                    if (par01[i] == par01[j])
                    {
                        op01 = op01 + op01[j].ToString();
                        matchfound = true;
                        //matchlocation = j;
                        break;
                    }
                    //else
                    //{
                    //    count++;
                    //    op01 = op01 + count.ToString();
                    //}
                }
                if (!matchfound)
                {
                    count++;
                    op01 = op01 + count.ToString();
                    //matchfound = false;
                }

            }
            Console.WriteLine(op01);
        }

        public static void GetSecondLargestNumber()
        {
            int[] inputArray = new int[] { 1, 5, 3, 4, 22, 1 };
            int largest = int.MinValue;
            int secondLargest = largest;

            foreach (int i in inputArray)
            {
                if (i > largest)
                {
                    secondLargest = largest;
                    largest = i;
                }
                else if (i > largest)
                {
                    secondLargest = i;
                }
            }

            Console.WriteLine(secondLargest);
        }
    }
}
