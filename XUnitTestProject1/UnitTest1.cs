using python;
using System;
using System.Globalization;
using Xunit;

namespace XUnitTestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void GetFreqWrongPath()
        {
            Assert.Equal("Erreur: File doesn't exist", PyUtils.Getfreq("test"));
        }

        [Fact]
        public void GetFreqWrongTypeOfFile()
        {
            Assert.Equal("Erreur: File with wrong extension", PyUtils.Getfreq(@"D:\programmation\python\TFE\note.py"));
        }

        [Fact]
        public void FreqToNoteNegatif()
        {
            var test = PyUtils.FreqToNote(Math.Abs(float.Parse("5.2", CultureInfo.InvariantCulture)));
            Assert.Equal("a", PyUtils.FreqToNote(5.2f));
        }
    }
}
