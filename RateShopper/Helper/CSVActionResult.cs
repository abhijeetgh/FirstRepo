using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RateShopper.Helper
{
    public sealed class CsvActionResult : FileResult
    {
        private readonly DataSet _dataSet;
        private readonly bool _skipFirstTableHeader = false;

        public CsvActionResult(DataSet dataSet, string fileName)
            : base("text/csv")
        {
            _dataSet = dataSet;
            FileDownloadName = fileName;            
        }

        public CsvActionResult(DataSet dataSet, string fileName, bool skipFirstTableHeader)
            : base("text/csv")
        {
            _dataSet = dataSet;
            FileDownloadName = fileName;
            _skipFirstTableHeader = skipFirstTableHeader;
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            if (response != null)
            {
                var outputStream = response.OutputStream;
                using (var memoryStream = new MemoryStream())
                {
                    WriteDataTable(memoryStream);
                    outputStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                }
            }
        }

        private void WriteDataTable(Stream stream)
        {
            var streamWriter = new StreamWriter(stream, Encoding.Default);
            foreach (DataTable _dataTable in _dataSet.Tables)
            {
                if (_dataTable.TableName == _dataSet.Tables[0].TableName)
                {
                    if (!_skipFirstTableHeader)
                    {
                        WriteHeaderLine(streamWriter, _dataTable);
                        streamWriter.WriteLine();
                    }
                }
                else
                {
                    WriteHeaderLine(streamWriter, _dataTable);
                    streamWriter.WriteLine();
                }
                WriteDataLines(streamWriter, _dataTable);
                streamWriter.WriteLine();
            }

            streamWriter.Flush();
        }

        private static void WriteHeaderLine(StreamWriter streamWriter, DataTable _dataTable)
        {
            foreach (DataColumn dataColumn in _dataTable.Columns)
            {
                WriteValue(streamWriter, dataColumn.ColumnName.Replace("_", " "));
            }
        }

        private static void WriteDataLines(StreamWriter streamWriter, DataTable _dataTable)
        {
            foreach (DataRow dataRow in _dataTable.Rows)
            {
                foreach (DataColumn dataColumn in _dataTable.Columns)
                {
                    WriteValue(streamWriter, dataRow[dataColumn.ColumnName].ToString());
                }
                streamWriter.WriteLine();
            }
        }


        private static void WriteValue(StreamWriter writer, String value)
        {
            writer.Write("\"");
            writer.Write(value.Replace("\"", "\"\""));
            writer.Write("\",");
        }
    }
}