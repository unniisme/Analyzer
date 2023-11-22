﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;

namespace Analyzer.Tests
{
    [TestClass()]
    public class AnalyzerTests
    {
        [TestMethod()]
        public void AnalyzerTest()
        {
            Analyzer analyzer = new();

            IDictionary<int , bool> teacherOptions = new Dictionary<int , bool>
            {
                [101] = true ,
                [102] = true ,
                [103] = true ,
                [104] = true ,
                [105] = true ,
                [106] = true ,
                [107] = true ,
                [108] = true ,
                [109] = true ,
                [110] = true ,
                [111] = true ,
                [112] = true ,
                [113] = true ,
                [114] = true ,
                [115] = true ,
                [116] = true
            };

            analyzer.Configure(teacherOptions);

            List<string> paths = new()
            {
                "..\\..\\..\\TestDLLs\\Abstract.dll"
            };

            analyzer.LoadDLLFileOfStudent(paths);

            Dictionary<string, List<AnalyzerResult>> result = analyzer.Run();

            Dictionary<string, List<AnalyzerResult>> original = new();

            original["Abstract.dll"] = new List<AnalyzerResult> { 

                new AnalyzerResult("101", 1, ""),
                new AnalyzerResult("102", 0, "Classes ClassLibrary1.Badname contains only static fields and methods, but has non -static, visible constructor.Try changing it to private or make it static."),
                new AnalyzerResult("103", 1, "No violation found."),
                new AnalyzerResult("104", 1, "No violation found."),
                new AnalyzerResult("105", 1, "Depth of inheritance rule followed by all classes."),
                new AnalyzerResult("106", 1, "Everything looks fine. No readonly array fields found."),
                new AnalyzerResult("107", 1, "Everything looks fine.No switch statements found."),
                new AnalyzerResult("108", 0, "No violations found."),
                new AnalyzerResult("109", 0, "No unused local variables found."),
                new AnalyzerResult("110", 0, "No occurrences of useless control flow found."),
                new AnalyzerResult("111", 0, "No Violation FoundIncorrect Abstract Class Naming : Badname"),
                new AnalyzerResult("112", 0, "No Violation FoundIncorrect Method Naming : .ctor"),
                new AnalyzerResult("113", 1, "No methods have cyclomatic complexity greater than 10"),
                new AnalyzerResult("114", 1, ""),
                new AnalyzerResult("115", 1, "No Violation Found"),
                new AnalyzerResult("116", 1, ""),
                new AnalyzerResult("117", 1, "No Violation Found")

            };

            foreach(KeyValuePair<string , List<AnalyzerResult>> dll in result)
            {
                Console.WriteLine(dll.Key);

                foreach(AnalyzerResult res in dll.Value)
                {
                    Console.WriteLine(res.AnalyserID + " " + res.Verdict + " " + res.ErrorMessage);
                }
            }

            Assert.AreEqual(result.ToString(), original.ToString());
        }
    }
}
