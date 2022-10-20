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
using System.Data;

namespace Deloitte.Ink.Transition.Importer.Reports.Common
{
    public abstract class Workflows : IWorkflows
    {
        private readonly ITransitionCoreServices _coreServices;

        /// <inheritdoc />
        public ITransitionCoreServices CoreServices => _coreServices;

        /// <inheritdoc />
        public ILocationService LocationService => _coreServices.LocationService;

        /// <inheritdoc />
        public ITransitionContextService TransitionContextService => _coreServices.TransitionContextService;

       
        protected Workflows(ITransitionCoreServices coreServices )
        {
            _coreServices = coreServices;
        }

        public abstract DataTable GeneratePublicationReport(bool isZipped);

        public abstract DataTable GenerateExcelReport(string inputValue);
       
    }
}
