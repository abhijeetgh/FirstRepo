using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Contracts.Dtos
{
    public class EmailGeneratorDto
    {

        public Nullable<int> SelectedReceipient { get; set; }

        public string CustomEmailAddress { get; set; }

        public IEnumerable<string> SelectedUserEmails { get; set; }

        public IEnumerable<string> EmailRecipients { get; set; }

        public int[] SelectedInfoToSend { get; set; }

        public InformationToSendDto InformationToSendDto { get; set; }

        public IEnumerable<NotesDto> SelectedNotes { get; set; }

        public int[] SelectedNotesToSend { get; set; }

        public IEnumerable<PicturesAndFilesDto> SelectedFiles { get; set; }

        public int[] SelectedFilesToSend { get; set; }

        public long ClaimId { get; set; }

        public string Remarks { get; set; }

        public string CompanyAbbr { get; set; }

        public string LoggedInUserEmail { get; set; }

        public string Subject { get; set; }
    }
}
