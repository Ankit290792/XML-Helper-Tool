using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deloitte.Ink.Transition.Importer.Reports.Interface
{
    public interface ITransitionCoreServices
    {
 

        /// <summary>
        /// Content Transition System working disk locations for the converter and importer
        /// </summary>
        ILocationService LocationService { get; }

        /// <summary>
        /// Service that contains the <see cref="TransitionContextFile"/>
        /// </summary>
        ITransitionContextService TransitionContextService { get; }

        /// <summary>
        /// FileInfo of the publication zip file from the Source directory, including the TransitionId prefix and zip file extension currently being processed.
        /// </summary>
        FileInfo PublicationTransitionFileInfo { get; set; }

        /// <summary>
        /// Original publication name that was uploaded, without the TransitionId prefix, and without the .zip file extension
        /// </summary>
        public string OriginalPublicationName { get; set; }

        /// <summary>
        /// Unzips package to working folder, finds the <see cref="TransitionContextFile"/>, and makes it available through the <see cref="ITransitionContextService"/>.
        /// This method will be used only by the importer.  The logic for unzipping and getting <see cref="TransitionContextFile"/> has been
        /// pushed down into the converter classes.
        /// </summary>
        /// <param name="packageName">Publication name that is to be transitioned</param>
        /// <returns>True if unzipped and Tran</returns>
        void Initialize();
       
    }
}
