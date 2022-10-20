using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Deloitte.Ink.Transition.Importer.Reports.Constants;
using Deloitte.Ink.Transition.Importer.Reports.Interface;
using Deloitte.Ink.Transition.Importer.Reports.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Deloitte.Ink.Transition.Importer.Reports.Services
{
    public class TransitionCoreServices : ITransitionCoreServices
    {
        //  private members
        private readonly ILogger<TransitionCoreServices> logger;
        public ILocationService LocationService { get; }

        /// <inheritdoc />
        public ITransitionContextService TransitionContextService { get; }

        /// <inheritdoc />
        public FileInfo PublicationTransitionFileInfo { get; set; }

        /// <inheritdoc />
        public string OriginalPublicationName { get; set; }

        public TransitionCoreServices(ILocationService locationService, ITransitionContextService transitionContextService, ILogger<TransitionCoreServices> _logger)
        {

            LocationService = locationService;
            TransitionContextService = transitionContextService;

            logger = _logger;
        }


        protected virtual TransitionContextFile GetTransitionContextFile()
        {
            var tctxDirectoryInfo = new DirectoryInfo(Path.Combine(LocationService.OutputDirectoryInfo.FullName, "cvt"));
            var tctxFileInfo = tctxDirectoryInfo.GetFiles($"*{FileExtensionConstants.TctxFile}");
            var transtionContextFile = File.ReadAllText(tctxFileInfo[0].FullName, new UTF8Encoding(true));
            return transtionContextFile.DeserializeJsonToObject<TransitionContextFile>();

        }
        public virtual void UnZipPackage(string importerPackageLocation, string extractPathDirectory)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var encoding = Encoding.GetEncoding(1252);
            if (!System.IO.Directory.Exists(extractPathDirectory))
                System.IO.Directory.CreateDirectory(extractPathDirectory);
            ZipFile.ExtractToDirectory(importerPackageLocation, extractPathDirectory);

        }
        public virtual void GetSourceAndConvertedPackage(bool isZipped, string sourceFileName, string cvtFileName)
        {
            if (isZipped)
            {
                logger.LogInformation("Reading the source file: " + sourceFileName);

                UnZipPackage(sourceFileName, LocationService.OutputDirectoryInfo.FullName + "\\src");
                logger.LogInformation("Unzipped the source package");


                UnZipPackage(cvtFileName, LocationService.OutputDirectoryInfo.FullName + "\\cvt");
                logger.LogInformation("Unzipped the converted package");
            }
        }

        void ITransitionCoreServices.Initialize()
        {
            TransitionContextService.AddTransitionContext(GetTransitionContextFile());
        }
    }
}
