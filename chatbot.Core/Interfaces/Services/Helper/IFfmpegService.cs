using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services.Helper
{
    public interface IFfmpegService
    {
        Task<string> RunFfprobeAsync( string arguments, CancellationToken cancellationToken = default);

        Task RunFfmpegAsync( string arguments,CancellationToken cancellationToken = default);
    }
}
