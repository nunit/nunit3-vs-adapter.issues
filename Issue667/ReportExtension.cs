using NUnit.Engine.Extensibility;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace NUnit.Extension.CustomResultWriter
{
    /// <summary>
    /// CustomResultWriter class
    /// </summary>
    [Extension, ExtensionProperty("Format", "myformat")]
    public class CustomResultWriter : IResultWriter
    {
        /// <summary>
        /// CheckWritability
        /// </summary>
        /// <param name="outputPath"></param>
        public void CheckWritability(string outputPath)
        {
            using (new StreamWriter(outputPath, false, Encoding.UTF8))
            { }
        }
        /// <summary>
        /// WriteResultFile
        /// </summary>
        /// <param name="resultNode"></param>
        /// <param name="outputPath"></param>
        public void WriteResultFile(XmlNode resultNode, string outputPath)
        {
            using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                WriteResultFile(resultNode, writer);
            }
        }
        /// <summary>
        /// WriteResultFile
        /// </summary>
        /// <param name="resultNode"></param>
        /// <param name="writer"></param>
        public void WriteResultFile(XmlNode resultNode, TextWriter writer)
        {
            var xmlSettings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "\t"
            };

            string directory = new FileInfo(((FileStream)((StreamWriter)writer).BaseStream).Name).DirectoryName;

            using (var x = XmlWriter.Create(writer, xmlSettings))
            {
                // header for complete result file
                WriteHeader(x, resultNode);

                foreach (XmlNode node in resultNode.SelectNodes("//test-case"))
                {
                    string testname = node.Attributes["name"].Value;

                    WriteTestcase(x, testname, node);

                    // write single logfile for each testcase
                    var singleTestFileName = new StringBuilder(directory);
                    singleTestFileName.Append("\\");
                    singleTestFileName.Append(testname);
                    singleTestFileName.Append(GetPostfixNumber(directory, testname));
                    singleTestFileName.Append(".xml");

                    using (var singleTestWriter = XmlWriter.Create(singleTestFileName.ToString(), xmlSettings))
                    {
                        WriteHeader(singleTestWriter, resultNode);
                        WriteTestcase(singleTestWriter, testname, node);
                        WriteFooter(singleTestWriter);
                    }
                }

                // footer for complete result file
                WriteFooter(x);
            }
        }


        private void WriteHeader(XmlWriter xmlWriter, XmlNode resultNode)
        {
            var testRun = resultNode.SelectSingleNode("//test-run");
            var environment = resultNode.SelectSingleNode("//environment");

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("testcampaign");                            // <testcampaign>
            xmlWriter.WriteAttributeString("total", testRun.Attributes["total"].Value);

            xmlWriter.WriteStartElement("environment");                             //   <environment>
            xmlWriter.WriteAttributeString("os", environment.Attributes["os-version"].Value);
            xmlWriter.WriteAttributeString("nunit-framework", environment.Attributes["framework-version"].Value);
            xmlWriter.WriteAttributeString("nunit-engine", testRun.Attributes["engine-version"].Value);
            xmlWriter.WriteEndElement();                                            //   </environment>
        }
        private void WriteFooter(XmlWriter xmlWriter)
        {
            xmlWriter.WriteEndElement();                                            // </testcampaign>
            xmlWriter.Flush();
        }
        private void WriteTestcase(XmlWriter xmlWriter, string testname, XmlNode node)
        {
            xmlWriter.WriteStartElement("testcase");                            //   <testcase>
            xmlWriter.WriteAttributeString("name", testname);
            xmlWriter.WriteAttributeString("result", node.Attributes["result"].Value);

            // convert utc to local time
            DateTime timestamp = DateTime.Parse(node.Attributes["start-time"].Value);
            string localTime = timestamp.ToLocalTime().ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
            xmlWriter.WriteAttributeString("date", localTime.Replace(" ", "-"));

            foreach (XmlNode subNode in node.ChildNodes)
            {
                // contains the test output (assert details and comments)
                if (subNode.Name.Equals("output"))
                {
                    xmlWriter.WriteRaw(subNode.InnerText.Replace("<![CDATA[", "").Replace("]]>", ""));
                }
            }

            xmlWriter.WriteRaw("\r\n\t");
            xmlWriter.WriteEndElement();                                        //   </testcase>
        }
        private string GetPostfixNumber(string directory, string testName)
        {
            int lastNumber = 0;

            foreach (var file in Directory.GetFiles(directory, testName + "*.xml"))
            {
                if (int.TryParse(System.Text.RegularExpressions.Regex.Match(file, "([0-9]{3})").Value, out int number))
                {
                    if (number > lastNumber)
                        lastNumber = number;
                }
            }

            return (lastNumber + 1).ToString("D3");
        }
    }
}
