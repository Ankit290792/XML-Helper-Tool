using Deloitte.Ink.Transition.Importer.Reports.Constants;
using Deloitte.Ink.Transition.Importer.Reports.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deloitte.Ink.Transition.Importer.Reports.Models
{
    public class TransitionContextFile
    {
        /// <summary>
        /// Contains the information for the operation of the Content Transition System
        /// </summary>
        public TransitionSystemInfo? TransitionSystemInfo { get; set; }
        /// <summary>
        /// Contains the content metadata needed for a transitioned publication
        /// </summary>
        public ContentMetadata? ContentMetadata { get; set; }

        /// <summary>
        /// Serialized to a Json string.
        /// </summary>
        /// <returns>A Json string.</returns>
        public override string ToString()
        {
            return this.SerializeObjectToJson();
        }
    }

    /// <summary>
    /// Contains the information for the operation of the Content Transition System
    /// </summary>
    public class TransitionSystemInfo
    {
        /// <summary>
        /// Guid that represents all activities by all components of the Content Transition System
        /// </summary>
        public string? TransitionId { get; set; }

        /// <summary>
        /// Represents the origin of the package being transitioned
        /// </summary>
        public string? SourceSystem { get; set; }

        /// <summary>
        /// Determined by the user to instruct the Content Transition System to perform only a conversion.  No import transformation or import into Docs will be performed
        /// </summary>
        public bool ConversionOnly { get; set; }

        /// <summary>
        /// Determined by the user to instruct the Content Transition System to perform a conversion and an import transformation only.  No import into Docs will be performed
        /// </summary>
        public bool ImportTransformationOnly { get; set; }

        /// <summary>
        /// User that performed a transition, a <see cref="TransitionSystemInfo.ConversionOnly"/> or a <see cref="TransitionSystemInfo.ImportTransformationOnly"/>
        /// </summary>
        public string? User { get; set; }

        /// <summary>
        /// Full name of the publication zip file that was uploaded by the user
        /// </summary>
        public string? PublicationZipFileUploaded { get; set; }

        /// <summary>
        /// DateTime transition was submitted
        /// </summary>
        public DateTime TransitionSubmittedTimeStamp { get; set; }

        /// <summary>
        /// The import behavior for an object that has already been imported, and is in a draft state in Docs.  <see cref="ImporterBehaviorConstants.IfObjectExistsDraftState"/>
        /// </summary>
        public string? IfObjectExistsDraftState { get; set; } 

        /// <summary>
        /// The import behavior for an object that has already been imported, and is in a released stated in Docs.  <see cref="ImporterBehaviorConstants.IfObjectExistsReleasedState"/>
        /// </summary>
        public string? IfObjectExistsReleasedState { get; set; }

        /// <summary>
        /// The import behavior for a publication that has already been imported.  <see cref="ImporterBehaviorConstants.IfPubExists"/>
        /// </summary>
        public string? IfPubExists { get; set; } 

        /// <summary>
        /// Import behavior that determines if baseline should be updated.
        /// </summary>
        public bool UpdateBaseline { get; set; } = true;

    }

    /// <summary>
    /// Contains the content metadata needed for a transitioned publication
    /// </summary>
    public class ContentMetadata
    {
        /// <summary>
        /// Folder location id in the Docs repository where 
        /// </summary>
        public string? TargetFolder { get; set; }

        /// <summary>
        /// Folder location in the Docs repository where 
        /// </summary>
        public string? TargetFolderDisplay { get; set; }

        /// <summary>
        /// Publication name
        /// </summary>
        public string? PublicationName { get; set; }

        /// <summary>
        /// Publication title
        /// </summary>
        public string? PublicationTitle { get; set; }

        /// <summary>
        /// Navigation title
        /// </summary>
        public string? NavigationTitle { get; set; }

        /// <summary>
        /// Applicable member firms for this publication
        /// </summary>
        public IList<string>? MemberFirm { get; set; }

        /// <summary>
        /// MemberFirm Display values
        /// </summary>
        public IList<string>? MemberFirmDisplay { get; set; }

        /// <summary>
        /// Applicable catalog for this publication
        /// </summary>
        public IList<string>? Catalog { get; set; }

        /// <summary>
        /// Applicable catalog display value for this publication
        /// </summary>
        public IList<string>? CatalogDisplay { get; set; }

        /// <summary>
        /// Applicable accounting framework for this publication
        /// </summary>
        public string? AccountingFramework { get; set; }

        /// <summary>
        ///  Applicable accounting framework display values for this publication
        /// </summary>
        public string? AccountingFrameworkDisplay { get; set; }

        /// <summary>
        /// Applicable auditing framework for this publication
        /// </summary>
        public IList<string>? AuditingFramework { get; set; }

        /// <summary>
        ///  Applicable auditing framework display values for this publication
        /// </summary>
        public IList<string>? AuditingFrameworkDisplay { get; set; }

        /// <summary>
        /// Publication type for this publication
        /// </summary>
        public string? PublicationType { get; set; }

        /// <summary>
        /// Publication type display values for this publication
        /// </summary>
        public string? PublicationTypeDisplay { get; set; }

        /// <summary>
        /// Folio auditing account for this publication
        /// </summary>
        public IList<string>? FolioAuditingAccount { get; set; }

        /// <summary>
        ///  FolioAuditingAccount display account for this publication
        /// </summary>
        public IList<string>? FolioAuditingAccountDisplay { get; set; }

        /// <summary>
        /// Folio auditing non-account for this publication
        /// </summary>
        public IList<string>? FolioAuditingNonAccount { get; set; }

        /// <summary>
        /// Folio non auditing display values non-account for this publication
        /// </summary>
        public IList<string>? FolioAuditingNonAccountDisplay { get; set; }


        /// <summary>
        /// Folio accounting for this publication
        /// </summary>
        public IList<string>? FolioAccounting { get; set; }

        /// <summary>
        /// Folio accounting display values for this publication
        /// </summary>
        public IList<string>? FolioAccountingDisplay { get; set; }

        /// <summary>
        /// Journal brand if applicable for this publication
        /// </summary>
        public string? JournalBrand { get; set; }

        /// <summary>
        /// Date publication was published
        /// </summary>
        public DateTime PublicationDate { get; set; }
    }




}
