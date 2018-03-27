using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Milou.Web.ClientResources.Tests.Integration
{
    public static class ExceptionExtensions
    {
        public static bool IsFatal(this Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }

            return
                ex is StackOverflowException ||
                ex is OutOfMemoryException ||
                ex is AccessViolationException ||
                ex is AppDomainUnloadedException ||
                ex is ThreadAbortException ||
                ex is SEHException;
        }

        public static Exception DeepestException(this Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }

            if (ex.InnerException == null)
            {
                return ex;
            }
            return DeepestException(ex.InnerException);
        }
    }
}