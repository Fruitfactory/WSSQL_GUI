﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSUI.Infrastructure.Controls.ProgressManager
{
    public interface IProgressManager
    {
        void StartOperation(ProgressOperation operation);
        void StopOperation();
        void SetProgress(int value);
    }
}