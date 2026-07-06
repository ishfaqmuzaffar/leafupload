using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LeafUpload.Infrastructure.Persistence
{
    public class InMemoryLeafRepository : ILeafRepository
    {
        private readonly ConcurrentDictionary<Guid, LeafSample> _samples = new();
        private readonly ConcurrentDictionary<Guid, DiagnosisResult> _diagnoses = new();

        public Task SaveSampleAsync(LeafSample sample)
        {
            _samples[sample.Id] = sample;
            return Task.CompletedTask;
        }

        public Task SaveDiagnosisAsync(DiagnosisResult diagnosis)
        {
            _diagnoses[diagnosis.Id] = diagnosis;
            return Task.CompletedTask;
        }

        public Task<LeafSample?> GetSampleByIdAsync(Guid id)
        {
            _samples.TryGetValue(id, out var sample);
            return Task.FromResult(sample);
        }

        public Task<DiagnosisResult?> GetDiagnosisByIdAsync(Guid id)
        {
            _diagnoses.TryGetValue(id, out var diagnosis);
            return Task.FromResult(diagnosis);
        }
    }
}
