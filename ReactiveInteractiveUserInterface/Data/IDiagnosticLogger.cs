using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data;

internal interface IDiagnosticLogger : IDisposable
{
    void LogBallState(IBall ball);
}
