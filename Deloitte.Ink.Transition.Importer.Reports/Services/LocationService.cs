using Deloitte.Ink.Transition.Importer.Reports.Interface;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deloitte.Ink.Transition.Importer.Reports.Services
{
    public class LocationService : ILocationService
    {
        private readonly LocationConfig _locationConfig;

       
        /// <inheritdoc />
        public DirectoryInfo InputDirectoryInfo { get; private set; }

        /// <inheritdoc />
        public DirectoryInfo OutputDirectoryInfo { get; set; }

        /// <inheritdoc />
        public DirectoryInfo ExportDirectoryInfo { get; private set; }

 

        // ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="locationConfig"><see cref="LocationConfig"/></param>
        public LocationService(IOptions<LocationConfig> locationConfig)
        {
            _locationConfig = locationConfig.Value;
        }

       

        public  void SetUpDirectories()
        {

            var rootDirectory = new DirectoryInfo("C:\\Temp\\ReportImporter");
            if (!rootDirectory.Exists)
                rootDirectory.Create();

            OutputDirectoryInfo = new DirectoryInfo(Path.Combine(rootDirectory.FullName, "ImpOutput"));
            if (!Directory.Exists(Path.Combine(rootDirectory.FullName, "ImpOutput")))
            {
                OutputDirectoryInfo.Create();
            }


            InputDirectoryInfo = new DirectoryInfo(Path.Combine(rootDirectory.FullName, "ImpInput"));
            if (!Directory.Exists(Path.Combine(rootDirectory.FullName, "ImpInput")))
            {
                InputDirectoryInfo.Create();
            }

            ExportDirectoryInfo = new DirectoryInfo(Path.Combine(rootDirectory.FullName, "ImpExport"));
            if (!Directory.Exists(Path.Combine(rootDirectory.FullName, "ImpExport")))
            {
                ExportDirectoryInfo.Create();
            }
        }

        /// <inheritdoc />
        public  void CleanUpDirectories(string? extractPathDirectory, string? importerPackageLocation)
        {
            //  DirectoryInfo directoryInfo = new DirectoryInfo(path: extractPathDirectory == null ? "" : extractPathDirectory);
            //  directoryInfo?.Delete(true);

            // directoryInfo = new DirectoryInfo(importerPackageLocation);
            // directoryInfo?.Delete(true);

            SetUpDirectories();

        }

       
    }
    public class LocationConfig
    {
        public string impOutputDirectory { get; set; }

     
        public string impInputDirectory { get; set; }

       
        public string exportLocation { get; set; }

        public string DirectoryRoot { get; set; }

    }
}
