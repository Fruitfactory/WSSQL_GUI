﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSUI.Infrastructure.Services
{
    public class EventArgs<T> : EventArgs
    {
        private T _value;

        public EventArgs(T value)
        {
            _value = value;
        }

        public T Value 
        { 
            get 
            { 
                return _value; 
            } 
        }

    }
}
