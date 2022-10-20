using Deloitte.Ink.Transition.Importer.Reports.Constants;
using Deloitte.Ink.Transition.Importer.Reports.Interface;
using Deloitte.Ink.Transition.Importer.Reports.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Deloitte.Ink.Transition.Importer.Reports.Services;
using System.Data;
using System.Xml.Linq;
using System.Xml.XPath;
using Deloitte.Ink.Transition.Importer.Reports.Extensions;

namespace Deloitte.Ink.Transition.Importer.Reports.Common
{
    public abstract class ContentReport<T> : CoreService, IReport<T>
    {
        private readonly ITransitionCoreServices _coreServices;
        public DirectoryInfo? SourceDirectoryInfo { get; private set; }
        public ILocationService LocationService => _coreServices.LocationService;

        public ITransitionContextService TransitionContextService => _coreServices.TransitionContextService;
        public ITransitionCoreServices CoreServices => _coreServices;
        public IWorkflowResolver WorkflowResolver { get; set; }
        public ILogger<T> Logger { get; }



        //  ctor
        protected ContentReport(ITransitionCoreServices coreServices, IWorkflowResolver workflowResolver, ILogger<T> logger)
        {
            _coreServices = coreServices;
            WorkflowResolver = workflowResolver;
            Logger = logger;
        }

        //  abstract methods
        public abstract Task ProcessAsync();

        protected virtual void InitializeReport(bool isZipped)
        {

            DirectoryInfo directoryInfo = LocationService.InputDirectoryInfo;
            try
            {
                if (Directory.Exists(directoryInfo.FullName))
                {
                    FileInfo[] Files = directoryInfo.GetFiles("*.zip");
                    if (Files != null && Files.Count() > 0)
                    {
                        var sourceFileName = Files.Where(x => !x.Name.ToLower().Contains("cvt=")).FirstOrDefault()?.FullName;
                        var cvtFileName = Files.Where(x => x.Name.ToLower().Contains("cvt=")).FirstOrDefault()?.FullName;


                        if (sourceFileName != null && cvtFileName != null)
                        {
                            GetSourceAndConvertedPackage(isZipped, sourceFileName, cvtFileName);

                        }
                        else
                        {

                            Logger.LogInformation("Not a valid package, place an importer package");
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.Message.ToString());
            }



        }

        public virtual void GetSourceAndConvertedPackage(bool isZipped, string sourceFileName, string cvtFileName)
        {
            if (isZipped)
            {
                Logger.LogInformation("Reading the source file: " + sourceFileName);

                UnZipPackage(sourceFileName, LocationService.OutputDirectoryInfo.FullName + "\\src");
                Logger.LogInformation("Unzipped the source package");


                UnZipPackage(cvtFileName, LocationService.OutputDirectoryInfo.FullName + "\\cvt");
                Logger.LogInformation("Unzipped the converted package");
            }
        }


    }


}
