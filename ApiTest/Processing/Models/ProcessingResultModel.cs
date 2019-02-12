using System.Collections.Generic;

namespace DotTest.Processing.Models
{
    /// <summary>
    /// Модель данных результата рассчета статистики
    /// </summary>
    class ProcessingResultModel
    {
        public ProcessingRequestModel request;
        public List<FrequencyResult> ru;
        public List<FrequencyResult> en;

        public ProcessingResultModel(ProcessingRequestModel _request, List<FrequencyResult> _en, List<FrequencyResult> _ru)
        {
            request = _request;
            ru = _ru;
            en = _en;
        }
    }
}
