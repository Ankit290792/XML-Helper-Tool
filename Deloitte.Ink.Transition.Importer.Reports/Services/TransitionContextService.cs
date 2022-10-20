using Deloitte.Ink.Transition.Importer.Reports.Interface;
using Deloitte.Ink.Transition.Importer.Reports.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deloitte.Ink.Transition.Importer.Reports.Services
{
    public class TransitionContextService : ITransitionContextService
    {
        private TransitionContextFile _transitionContextFile;

        /// <inheritdoc />
        public string TransitionId { get; private set; }

        /// <inheritdoc />
        public TransitionContextFile TransitionContextFile => _transitionContextFile;

        /// <inheritdoc />
        public TransitionSystemInfo TransitionSystemInfo => _transitionContextFile.TransitionSystemInfo;

        /// <inheritdoc />
        public ContentMetadata ContentMetadata => _transitionContextFile.ContentMetadata;

        /// <summary>
        /// ctor
        /// </summary>
        public TransitionContextService()
        {
        }

        /// <inheritdoc />
        public void AddTransitionContext(TransitionContextFile transitionContextFile)
        {
            _transitionContextFile = transitionContextFile;
            TransitionId = _transitionContextFile.TransitionSystemInfo.TransitionId;
        }
    }
}
