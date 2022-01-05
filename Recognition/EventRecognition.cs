using Electronic_Organizer_API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Electronic_Organizer_API.Recognition
{
    public class EventRecognition
    {
        public static string ShowAsJsonString(string text)
        {
            string test = " "+"JANUARY\n01 saturday\nColoring 12:00 - 12:30\n-\nDecolorization 12:30-13:00\nMen's haircut 13:00 - 13:20\n" +" ";

            Console.WriteLine(text);
            var year = "2022";
            string[] lines;
            string newLineReg = "\n";
            string firstWordReg = @"^([a-zA-Z\D+]+)";
            string eventReg = @"^([a-zA-Z\D+]+)\s?([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]\s?-\s?([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$";
            string integerReg = @"\d+";
            string startTimeReg = @"(([a-zA-Z\D+]+)\s?)(.*)(\s?-\s?)";
            string endTimeReg = @"(\s?-\s?)(.*)";
            int count = 0;

            lines = text.Split(newLineReg);// split the lines
            var recognizedMonth = lines[0];
            var recognizedDay = Regex.Match(lines[1], integerReg).Value;
            var date = recognizedDay + '-' + recognizedMonth + '-' + year;
            List<RecognizedEvent> eventsList = new List<RecognizedEvent>();

            for (int i = 2; i < lines.Length; i++)
            {
                if (Regex.Match(lines[i], eventReg).Value != "")
                {
                    var recognizedStartTime = Regex.Match(lines[i], startTimeReg).Groups[3].Value;
                    var recognizedEndTime = Regex.Match(lines[i], endTimeReg).Groups[2].Value;
                    var recognizedName = Regex.Match(lines[i], firstWordReg).Value;
                    DateTime convertedStartTime = DateTime.Parse(date + ' ' + recognizedStartTime);
                    DateTime convertedEndTime = DateTime.Parse(date + ' ' + recognizedEndTime);
                    DateTime convertedDate = DateTime.Parse(date);
                    eventsList.Add(new RecognizedEvent { Id = count, Name = recognizedName, StartTime = convertedStartTime, EndTime = convertedEndTime, Date = convertedDate });
                    ++count;
                }
            }
            var json = JsonConvert.SerializeObject(eventsList);
            return json;
        }
    }
}
