using LeafUpload.Core.Models;
using System;
using System.Threading.Tasks;

namespace LeafUpload.Core.Abstractions
{
    public interface ILeafRepository
    {
        Task SaveSampleAsync(LeafSample sample);
        Task SaveDiagnosisAsync(DiagnosisResult diagnosis);

        Task<LeafSample?> GetSampleByIdAsync(Guid id);
        Task<DiagnosisResult?> GetDiagnosisByIdAsync(Guid id);
    }
}
