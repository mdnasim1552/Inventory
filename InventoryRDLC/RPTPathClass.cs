using Microsoft.Reporting.NETCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InventoryRDLC
{
    public static class RPTPathClass
    {
        public static Stream GetReportFilePath(string path)
        {
            Assembly assembly = typeof(RPTPathClass).Assembly;

            // Construct the full resource name
            string resourceName = $"{assembly.GetName().Name}.{path}.rdlc";

            // Get the report file as an embedded resource stream
            Stream stream = assembly.GetManifestResourceStream(resourceName);

            return stream;

            //var assamblyPath = Assembly.GetExecutingAssembly().CodeBase;
            //Assembly assembly1 = Assembly.LoadFrom(assamblyPath);
            //Stream stream1 = assembly1.GetManifestResourceStream("RealERPRDLC." + path + ".rdlc");//R_24_CC.CancellationAddWork
            //return stream1;
        }

        public static LocalReport SetRDLCReportDatasets(this LocalReport report, Dictionary<string, object> datasets = null)
        {
            if (datasets != null)
            {
                foreach (var dataset in datasets)
                {
                    report.DataSources.Add(new ReportDataSource(dataset.Key, dataset.Value));
                }
            }

            return report;
        }
    }
}
