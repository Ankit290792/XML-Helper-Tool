using Deloitte.Ink.Transition.Importer.Reports.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deloitte.Ink.Transition.Importer.Reports.Interface
{
    public interface IReport<out T>
    {
        ILocationService LocationService { get; }

        ITransitionContextService TransitionContextService { get; }
        public ILogger<T> Logger { get; }

        Task ProcessAsync();
    }


    public interface ILocationService
    {


        /// <summary>
        /// Directory where original source to be processed is placed
        /// </summary>
        DirectoryInfo InputDirectoryInfo { get; }

        /// <summary>
        /// Directory used to unzip source package.  This is the directory where the <see cref="TransitionContextFile"/> resides.
        /// </summary>
        DirectoryInfo OutputDirectoryInfo { get; }

        /// <summary>
        /// Directory where processed files are placed until ready to be zipped
        /// </summary>
        DirectoryInfo ExportDirectoryInfo { get; }


        /// <summary>
        /// Ensures that a set of unique directories on the Windows File System exist, named uniquely by using the TransitionId"/>
        /// </summary>
        /// <param name="transitionId">Unique Transition Id</param>
        public abstract void SetUpDirectories();

        /// <summary>
        /// Deletes the Source, Working, Output and Completed directories and their contents.
        /// </summary>
        public abstract void CleanUpDirectories(string? extractPathDirectory, string? importerPackageLocation);
    }

    public interface ITransitionContextService
    {
        /// <summary>
        /// Unique transition id
        /// </summary>
        string TransitionId { get; }

        /// <summary>
        /// The information that contains Content Transition System operating information and content metadata available <see cref="TransitionContextFile"/>
        /// </summary>
        TransitionContextFile TransitionContextFile { get; }

        /// <summary>
        /// Information that contains the Transition Id and other Content Transition System operating data. The <see cref="TransitionSystemInfo"/> is also part of the <see cref="TransitionContextFile"/>
        /// </summary>
        TransitionSystemInfo TransitionSystemInfo { get; }

        /// <summary>
        /// The metadata chosen to associate to the publication being transitioned.  The <see cref="ContentMetadata"/> is also part of the <see cref="TransitionContextFile"/>
        /// </summary>
        ContentMetadata ContentMetadata { get; }

        /// <summary>
        /// Adds the <see cref="TransitionContextFile"/> to the TransitionContextService
        /// </summary>
        /// <param name="transitionContextFile"><see cref="TransitionContextFile"/></param>
        void AddTransitionContext(TransitionContextFile transitionContextFile);
    }
}
