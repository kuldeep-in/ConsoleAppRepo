
namespace ConsoleApp01.DataStructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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

        public string ReverseString01(string _title)
        {
            string result = "";

            if (string.IsNullOrEmpty(_title))
                return string.Empty;

            for (int i = _title.Length - 1; i >= 0; i--)
            {
                result += _title[i];
            }
            return result;
        }

        // Reverse each word of the sentence  
        public string ReverseEachString(string _title)
        {
            string result = "";
            if (string.IsNullOrEmpty(_title))
                return string.Empty;

            string[] arr = _title.Split(" ");
            for (int i = 0; i < arr.Length; i++)
            {
                if (i != arr.Length - 1)
                {
                    result += ReverseString01(arr[i]) + " ";
                }
                else
                {
                    result += ReverseString01(arr[i]) + " ";
                }
            }

            return result;
        }

        // Check the string is a palindrome  
        public bool CheckPalindrome(string _title)
        {
            bool result = true;

            if (string.IsNullOrEmpty(_title))
                return false;

            _title = _title.ToLower().Trim();

            var min = 0;
            var max = _title.Length - 1;

            while (max >= 0)
            {
                if (_title[min] == _title[max])
                {
                    min++;
                    max--;
                }
                else
                {
                    return false;
                }
            }

            return result;
        }

        // Check the max occurance of the any character in the string  
        public char? CheckMaxOccuranceOfChar(string _title)
        {
            char? maxOccuranceChar = null;
            int maxOccuranceValue = 0;

            if (string.IsNullOrEmpty(_title))
                return null;

            _title = _title.ToLower().Trim();
            char[] arr = _title.ToCharArray();

            Dictionary<char, int> _dictionary = new Dictionary<char, int>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != ' ')
                {
                    if (!_dictionary.ContainsKey(arr[i]))
                    {
                        _dictionary.Add(arr[i], 1);
                    }
                    else
                    {
                        _dictionary[arr[i]]++;
                    }
                }
            }

            foreach (KeyValuePair<char, int> item in _dictionary)
            {
                if (item.Value > maxOccuranceValue)
                {
                    maxOccuranceChar = item.Key;
                    maxOccuranceValue = item.Value;
                }
            }

            return maxOccuranceChar;
        }

        // Get the possible substring in a string  
        public void GetPossibleSubstring(string word)
        {
            if (!string.IsNullOrEmpty(word))
            {
                for (int i = 1; i < word.Length; i++)
                {
                    for (int j = 0; j <= word.Length - i; j++)
                    {
                        Console.WriteLine(word.Substring(j, i));
                    }
                }

                Console.ReadLine();
            }
        }

        public static void Comparestring(string par01, string par02)
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

        static long TotalAllEvenNumbers(int[] intArray)
        {
            return intArray.Where(i => i % 2 == 0).Sum(i => (long)i);
        }

        //Find the duplicate character  
        public StringBuilder GetDuplicateCharacter(string _title)
        {
            StringBuilder result = new StringBuilder();
            StringBuilder duplicateChar = new StringBuilder();

            foreach (var item in _title)
            {
                if (result.ToString().IndexOf(item.ToString().ToLower()) == -1)
                {
                    result.Append(item);
                }
                else
                {
                    duplicateChar.Append(item);
                }
            }

            return duplicateChar;
        }

    }
}
