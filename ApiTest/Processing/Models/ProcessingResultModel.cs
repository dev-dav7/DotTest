using System.Collections.Generic;

namespace DotTest.Processing.Models
{
    /// <summary>
    /// Модель данных результата рассчета статистики
    /// </summary>
    class ProcessingResultModel
    {
        ProcessingRequestModel request;
        List<FrequencyElement> frequencys;

        public ProcessingResultModel(ProcessingRequestModel _request, List<FrequencyElement> _frequencys)
        {
            request = _request;
            frequencys = _frequencys;
        }
    }
}
