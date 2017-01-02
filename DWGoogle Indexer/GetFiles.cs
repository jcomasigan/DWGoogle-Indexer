using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace DWGoogle
{
    class GetFiles
    {
        public static DataTable GetDwgs(string drive)
        {
            DataTable dwgsDataTable = FormatDataTable();
            string[] dir = Directory.GetDirectories(drive);
            string[] dwgs = Directory.GetFiles(drive, "*.DWG", SearchOption.TopDirectoryOnly);
            foreach (string dwg in dwgs)
            {
                DateTime modified = File.GetLastWriteTime(dwg);
                dwgsDataTable.Rows.Add(dwg, modified);
                ReadDwg.ParseDwg(dwg);
            }
            foreach (string subDir in dir)
            {
                string[] subsubdir = Directory.GetDirectories(subDir);

                foreach(string folder in subsubdir)
                {
                    dwgs = Directory.GetFiles(folder, "*.DWG", SearchOption.TopDirectoryOnly);
                    foreach(string dwg in dwgs)
                    {
                        DateTime modified = File.GetLastWriteTime(dwg);
                        dwgsDataTable.Rows.Add(dwg, modified);
                        ReadDwg.ParseDwg(dwg);
                    }
                }
            }
            return dwgsDataTable;
        }

        private static DataTable FormatDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Path", typeof(string));
            dt.Columns.Add("Modified", typeof(DateTime));
            return dt;
        }
    }
}
