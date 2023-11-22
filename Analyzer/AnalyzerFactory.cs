﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer
{
    public static class AnalyzerFactory
    {
        public static List<Tuple<int, string>> GetAllConfigurationOptions()
        {
            List<Tuple<int, string>> analyzers = new()
            {
                Tuple.Create(101, "Abstract type no public constructor"),
                Tuple.Create(102, "Avoid constructor in static types"),
                Tuple.Create(103, "Avoid unused private fields"),
                Tuple.Create(104, "Avoid empty interface"),
                Tuple.Create(105, "Depth of inheritance should be less than 3"),
                Tuple.Create(106, "Array field should not be read only"),
                Tuple.Create(107, "Avoid switch statements"),
                Tuple.Create(108, "Disposable field should be disposed"),
                Tuple.Create(109, "Avoid unused local variables"),
                Tuple.Create(110, "Useless control flow rule"),
                Tuple.Create(111, "Abstract class naming checker"),
                Tuple.Create(112, "Casing Checker"),
                Tuple.Create(113, "Cyclomatic Complexity"),
                Tuple.Create(114, "New Linelteral Rule"),
                Tuple.Create(115, "Prefix checker"),
                Tuple.Create(116, "Switch Statement default case checker"),
                Tuple.Create(117, "Avoid goto statements")
            };

            return analyzers;
        }

        public static IAnalyzer GetAnalyzer()
        {

            IAnalyzer Analyzer = new Analyzer();

            return Analyzer;
        }

    }
}