using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TestApp
{
    public interface IBarFactory
    {
        IBar Create(IBar instance);
    }

    public interface IBar
    {
        void ImVoid();
        int ImPassThrough(int goesin);      
    }

    public class TestImpl : IBar
    {
        public void ImVoid()
        {
            Trace.WriteLine("TestImpl");
        }

        public int ImPassThrough(int goesin)
        {
            Trace.WriteLine("ImPassThrough");
            return goesin;
        }
    }
}
