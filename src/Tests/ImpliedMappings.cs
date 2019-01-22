﻿namespace Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using SpreadsheetHelper;
    using SpreadsheetHelper.Configuration;
    using Xunit;

    public class ImpliedMappings
    {
        private const string unspecifiedIndexesSheetName = "unspecified indexes";

        private const int recordCount = 25;
        private readonly string filepath;

        public ImpliedMappings()
        {
            var folderPath = Path.Combine(Environment.CurrentDirectory, "test_outputs");
            var directory = Directory.CreateDirectory(folderPath);
            this.filepath = Path.Combine(folderPath, "implied_mappings.xlsx");
            if (File.Exists(this.filepath))
            {
                File.Delete(this.filepath);
            }

            var records = CreateTestRecords(recordCount);
            using (var spreadsheet = new Spreadsheet(this.filepath))
            {
                spreadsheet.WriteWorksheet<TestClass, TestClassMapUnspecifiedIndexes>(unspecifiedIndexesSheetName, records);
            }
        }

        [Fact]
        public void TestUnspecifiedIndexesValidation()
        {
            using (var spreadsheet = new Spreadsheet(this.filepath))
            {
                foreach (var record in spreadsheet.ReadWorksheet<TestClass, TestClassMapUnspecifiedIndexes>(unspecifiedIndexesSheetName))
                {
                    Assert.Equal("1", record.TestData1);
                    Assert.Equal("2", record.TestData2);
                    Assert.Equal("3", record.TestData3);
                    Assert.Equal("4", record.TestData4);
                    Assert.Equal("5", record.TestData5);
                }

                foreach (var record in spreadsheet.ReadWorksheet<TestClass, TestClassMapImpliedIndexesRead>(unspecifiedIndexesSheetName))
                {
                    Assert.Equal("1", record.TestData1);
                    Assert.Equal("2", record.TestData2);
                    Assert.Equal("3", record.TestData3);
                    Assert.Equal("4", record.TestData4);
                    Assert.Equal("5", record.TestData5);
                }
            }
        }

        [Fact]
        public void TestWrite()
        {
            var validator = new SpreadsheetValidator();
            validator.Validate(this.filepath);

            Assert.False(validator.HasErrors);
        }

        private static IEnumerable<TestClass> CreateTestRecords(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new TestClass();
            }
        }

        internal class TestClass
        {
            public string TestData1 { get; set; }
            public string TestData2 { get; set; }
            public string TestData3 { get; set; }
            public string TestData4 { get; set; }
            public string TestData5 { get; set; }
        }

        internal class TestClassMapImpliedIndexesRead : ClassMap<TestClass>
        {
            public TestClassMapImpliedIndexesRead()
            {
                Map(x => x.TestData5);
                Map(x => x.TestData2);
                Map(x => x.TestData1);
                Map(x => x.TestData3);
                Map(x => x.TestData4);
            }
        }

        internal class TestClassMapUnspecifiedIndexes : ClassMap<TestClass>
        {
            public TestClassMapUnspecifiedIndexes()
            {
                Map(x => x.TestData5).IndexWrite(5).ConstantWrite("5");
                Map(x => x.TestData2).ConstantWrite("2");
                Map(x => x.TestData1).IndexWrite(1).ConstantWrite("1");
                Map(x => x.TestData3).ConstantWrite("3");
                Map(x => x.TestData4).ConstantWrite("4");
            }
        }
    }
}