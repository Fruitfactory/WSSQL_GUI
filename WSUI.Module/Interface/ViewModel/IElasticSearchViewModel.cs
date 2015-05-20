﻿using System;

namespace OF.Module.Interface.ViewModel
{
    public interface IElasticSearchViewModel
    {

        void Initialize();

        object View { get; }

        bool IsServiceInstalled { get; }
        bool IsServiceRunning { get; }
        bool IsIndexExisted { get; }

        bool IsInitialIndexinginProgress { get; }

        void Show(bool showJustProgress);
        void Close();

        event EventHandler IndexingStarted;
        event EventHandler IndexingFinished;
    }
}